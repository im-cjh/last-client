using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Protocol;
using Unity.Collections;
using System.Threading.Tasks;

public class TowerPlacer : MonoBehaviour
{
    private Tilemap tilemap;
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>(); // 설치할 타워 프리팹
    [SerializeField] private float maxPlacementDistance = 5f; // 설치 가능한 최대 거리

    [SerializeField] private GameObject isValidTile; // 설치 가능한 타일 색상
    [SerializeField] private GameObject isUnvalidTile; // 설치 불가능한 타일 색상
    private GameObject currentHighlight;
    private Vector3Int previousCellPosition = Vector3Int.zero;

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
    }

    private async Task RegisterPrefab(string key)
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

    private string ExtractShortKey(string key)
    {
        // �����÷� �и��Ͽ� ������ �κи� ��ȯ
        return key.Substring(key.LastIndexOf('/') + 1);
    }

    void Update()
    {
        if (TowerPlacementManager.instance.IsPlacementActive())
        {
            HighlightTile();
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭
            {
                PlaceTower("BasicTower");
                Destroy(currentHighlight);
            }
        }

    }    

    private void PlaceTower(string prefabId)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 Cell 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        // 클릭한 셀에 타일이 있는지 확인
        if (tilemap.HasTile(cellPosition))
        {
            // 셀의 중심 월드 좌표 계산
            Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

            Collider2D[] hitcolliders = Physics2D.OverlapPointAll(cellCenterWorld, LayerMask.GetMask("Tower", "Character", "Enemy", "Obstacle"));
            if (hitcolliders.Length > 0)
            {
                Debug.Log("설치할 수 없는 위치입니다.");
                return;
            }

            // 캐릭터와의 거리 계산
            float distance = Vector3.Distance(transform.position, cellCenterWorld);
            if (distance > maxPlacementDistance)
            {
                Debug.Log("거리가 너무 멉니다.");
                return;
            }

            // 최대 거리 안이면 타워 생성 요청 보냄
            SendBuildRequestToServer(prefabId, cellPosition.x, cellPosition.y);

            // Instantiate(towerPrefabs[towerNum], cellCenterWorld, Quaternion.identity);
            // Debug.Log($"타워가 {cellPosition} 위치에 설치되었습니다.");
            // TowerPlacementManager.instance.SetPlacementState(false);
        }
    }

    private void SendBuildRequestToServer(string prefabId, float x, float y)
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
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_TowerBuildRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

    public void BuildTower(TowerData towerData)
    {
        Vector2 cellCenterWorld = new Vector2(towerData.TowerPos.X, towerData.TowerPos.Y);

        Instantiate(prefabMap[towerData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"타워가 {cellCenterWorld} 위치에 설치되었습니다.");
        TowerPlacementManager.instance.SetPlacementState(false);
    }

    private void HighlightTile()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 Cell 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        // 마우스가 새로운 타일에 위치했을 때만 처리
        if (cellPosition != previousCellPosition)
        {
            // 기존 하이라이트 제거
            if (currentHighlight != null)
            {
                Destroy(currentHighlight);
            }

            // 현재 타일을 하이라이트
            if (tilemap.HasTile(cellPosition))
            {
                Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

                Collider2D towerHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Tower"));
                Collider2D characterHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Character"));
                Collider2D enemyHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Enemy"));
                Collider2D obstacleHitCollider = Physics2D.OverlapPoint(cellCenterWorld, LayerMask.GetMask("Obstacle"));

                float distance = Vector3.Distance(transform.position, cellCenterWorld);

                // 설치 가능 여부에 따라 적절한 하이라이트 생성
                if (towerHitCollider == null && characterHitCollider == null && enemyHitCollider == null &&
                    obstacleHitCollider == null && distance <= maxPlacementDistance)
                {
                    currentHighlight = Instantiate(isValidTile, cellCenterWorld, Quaternion.identity);
                }
                else
                {
                    currentHighlight = Instantiate(isUnvalidTile, cellCenterWorld, Quaternion.identity);
                }
            }

            // 새로운 타일 위치 업데이트
            previousCellPosition = cellPosition;
        }
    }
}