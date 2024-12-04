using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Protocol;
using Unity.Collections;
using System.Threading.Tasks;

public class TowerPlacer : MonoBehaviour
{
    protected Tilemap tilemap;
    protected Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>(); // 설치할 타워 프리팹
    private string currentTowerPrefabId;
    protected string currentCardId;
    [SerializeField] protected float maxPlacementDistance = 5f; // 설치 가능한 최대 거리

    [SerializeField] protected GameObject isValidTile; // 설치 가능한 타일 색상
    [SerializeField] protected GameObject isUnvalidTile; // 설치 불가능한 타일 색상
    protected GameObject currentHighlight;
    protected Vector3 previousCellPosition = Vector3.zero;

    public static TowerPlacer instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    async void Start()
    {
        tilemap = Utilities.FindAndAssign<Tilemap>("Grid/Tile");
        await RegisterPrefab("Prefab/Towers/BasicTower");
        await RegisterPrefab("Prefab/Towers/BuffTower");
        await RegisterPrefab("Prefab/Towers/IceTower");
        await RegisterPrefab("Prefab/Towers/MissileTower");
        await RegisterPrefab("Prefab/Towers/StrongTower");
        await RegisterPrefab("Prefab/Towers/TankTower");
        await RegisterPrefab("Prefab/Towers/ThunderTower");
    }

    protected async Task RegisterPrefab(string key)
    {
        // Ű�� ����ȭ (��: Prefab/Enemy/Robot1 -> Robot1)
        string shortKey = ExtractShortKey(key);

        // �̹� ��ϵ� �������� ����
        if (prefabMap.ContainsKey(shortKey))
        {
            Debug.LogWarning($"Prefab '{shortKey}' is already registered.");
            return;
        }

        // ������ �ε�
        GameObject prefab = await AssetManager.LoadAsset<GameObject>(key);

        if (prefab != null)
        {
            prefabMap[shortKey] = prefab;
            //Debug.Log($"Prefab '{shortKey}' loaded and registered.");
        }
        else
        {
            Debug.LogError($"Failed to load prefab: {key}");
        }
    }

    protected string ExtractShortKey(string key)
    {
        // �����÷� �и��Ͽ� ������ �κи� ��ȯ
        return key.Substring(key.LastIndexOf('/') + 1);
    }

    void Update()
    {
        if (TowerPlacementManager.instance.IsPlacementActive())
        {
            currentTowerPrefabId = TowerPlacementManager.instance.GetTowerPrefabId();
            currentCardId = TowerPlacementManager.instance.GetCardId();

            HighlightTile();
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭
            {
                PlaceTower(currentTowerPrefabId, currentCardId);
                Destroy(currentHighlight);
            }
        }
    }

    private void PlaceTower(string prefabId, string cardId)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 Cell 좌표로 변환
        Vector3Int cellPosition = new Vector3Int((int)mouseWorldPos.x, (int)mouseWorldPos.y);
        Vector3Int worldCellPosition = tilemap.WorldToCell(cellPosition);

        // 클릭한 셀에 타일이 있는지 확인
        if (tilemap.HasTile(worldCellPosition))
        {
            // 셀의 중심 월드 좌표 계산
            //Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

            //Collider2D[] hitcolliders = Physics2D.OverlapPointAll(cellCenterWorld, LayerMask.GetMask("Tower", "Character", "Enemy", "Obstacle"));
            Collider2D[] hitcolliders = Physics2D.OverlapPointAll(mouseWorldPos, LayerMask.GetMask("Tower", "Character", "Enemy", "Obstacle"));
            if (hitcolliders.Length > 0)
            {
                Debug.Log("설치할 수 없는 위치입니다.");
                return;
            }

            // 캐릭터와의 거리 계산
            float distance = Vector2.Distance(transform.position, mouseWorldPos);
            //float distance = Vector3.Distance(transform.position, cellCenterWorld);
            if (distance > maxPlacementDistance)
            {
                Debug.Log("거리가 너무 멉니다.");
                Debug.Log(transform.position);
                Debug.Log(mouseWorldPos);
                Debug.Log(cellPosition);
                Debug.Log(worldCellPosition);
                return;
            }

            // 최대 거리 안이면 타워 생성 요청 보냄
            SendBuildRequestToServer(prefabId, cardId, cellPosition.x, cellPosition.y);
            //SendBuildRequestToServer(prefabId, mouseWorldPos.x, mouseWorldPos.y);
        }
        else
        {
            Debug.Log("타일이 없음" + cellPosition);

        }
    }

    private void SendBuildRequestToServer(string prefabId, string uuid, float x, float y)
    {
        // tower의 uuid는 서버에서 만들어서 보내줌
        Debug.Log($"서버에게 타워 설치 요청: prefabId:{prefabId}, x:{x}, y:{y}");
        Protocol.C2B_TowerBuildRequest pkt = new Protocol.C2B_TowerBuildRequest
        {
            Tower = new Protocol.TowerData
            {
                TowerPos = new Protocol.PosInfo
                {
                    X = x,
                    Y = y,
                },
                PrefabId = prefabId
            },
            RoomId = PlayerInfoManager.instance.roomId,
            OwnerId = PlayerInfoManager.instance.userId,
            CardId = uuid,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_TowerBuildRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

    public void BuildTower(TowerData towerData)
    {
        //Vector2 cellCenterWorld = new Vector2(towerData.TowerPos.X + 0.5f, towerData.TowerPos.Y + 0.5f);
        Vector2 cellCenterWorld = new Vector2(towerData.TowerPos.X, towerData.TowerPos.Y);

        GameObject newTower = Instantiate(prefabMap[towerData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"타워가 {cellCenterWorld} 위치에 설치되었습니다.");

        Tower towerScript = newTower.GetComponent<Tower>();
        if (towerScript != null)
        {
            towerScript.SetTowerId(towerData.TowerPos.Uuid);
            TowerManager.instance.AddTower(towerData.TowerPos.Uuid, towerScript);
        }
        
        TowerPlacementManager.instance.SetPlacementState(false, null, null);
    }

    private void HighlightTile()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Tile"));

        if (hit.collider != null)
        {
            // collider와 충돌한 위치
            Vector3 hitPoint = hit.point;

            Vector3 snappedPosition = new Vector3(
                Mathf.Floor(hitPoint.x),
                Mathf.Floor(hitPoint.y),
                0
            );

            // 마우스가 새로운 타일에 위치했을 때만 처리
            if (snappedPosition != previousCellPosition)
            {
                // 기존 하이라이트 제거
                if (currentHighlight != null)
                {
                    Destroy(currentHighlight);
                }

                float distance = Vector3.Distance(transform.position, snappedPosition);
                // 설치 가능 여부에 따라 적절한 하이라이트 생성
                if (distance <= maxPlacementDistance)
                {
                    currentHighlight = Instantiate(isValidTile, snappedPosition, Quaternion.identity);
                }
                else
                {
                    currentHighlight = Instantiate(isUnvalidTile, snappedPosition, Quaternion.identity);
                }

                // 새로운 타일 위치 업데이트
                previousCellPosition = snappedPosition;
            }
        }
    }
}