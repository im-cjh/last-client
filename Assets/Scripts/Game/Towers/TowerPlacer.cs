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

    [SerializeField] protected GameObject validTile; // 설치 가능한 타일 색상
    [SerializeField] protected GameObject unvalidTile; // 설치 불가능한 타일 색상
    protected bool isValidTile;
    protected GameObject currentHighlight;
    protected Vector3Int previousCellPosition = Vector3Int.zero;


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

        // 부모 오브젝트에서 카메라 컴포넌트를 가져옴
        Camera parentCamera = transform.parent.GetComponent<Camera>();
        if (parentCamera != null)
        {
            Debug.Log("Parent Camera found: " + parentCamera.name);
        }
        else
        {
            Debug.LogWarning("No Camera component found on the parent object.");
        }
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

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            if (cellPosition != previousCellPosition)
            {
                HighlightTile(cellPosition);
                previousCellPosition = cellPosition;
            }

            if (Input.GetMouseButtonDown(0) && isValidTile) // 마우스 클릭
            {
                PlaceTower(currentTowerPrefabId, currentCardId, cellPosition);
            }
        }
    }

    private void PlaceTower(string prefabId, string cardId, Vector3Int cellPosition)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
        worldPosition += offset;

        //부모 오브젝트의 Cam을 가져오기
        //Vector3 mouseWorldPos = parentCamera.ScreenToWorldPoint(Input.mousePosition);


        SendBuildRequestToServer(prefabId, cardId, worldPosition.x, worldPosition.y);

        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
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
        Vector3 cellCenterWorld = new Vector3(towerData.TowerPos.X, towerData.TowerPos.Y);

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

    private void HighlightTile(Vector3Int cellPosition)
    {
        // 기존 하이라이트 제거
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }

        // 유효한 타일인지 검사
        isValidTile = CheckPlacement(cellPosition);

        // 적절한 하이라이트 생성
        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
        worldPosition += offset;

        currentHighlight = Instantiate(isValidTile ? validTile : unvalidTile, worldPosition, UnityEngine.Quaternion.identity);
    }

    bool CheckPlacement(Vector3Int cellPosition)
    {
        if (!tilemap.HasTile(cellPosition))
        {
            return false;
        }

        Collider2D[] hitcolliders = Physics2D.OverlapPointAll(tilemap.GetCellCenterWorld(cellPosition), LayerMask.GetMask("Tower", "Character", "Enemy", "Obstacle"));
        if (hitcolliders.Length > 0)
        {
            return false;
        }

        Vector3 cellworldPosition = tilemap.GetCellCenterWorld(cellPosition);
        float distance = Vector3.Distance(transform.position, cellworldPosition);

        if (distance > maxPlacementDistance)
        {
            return false;
        }

        return true;
    }
}