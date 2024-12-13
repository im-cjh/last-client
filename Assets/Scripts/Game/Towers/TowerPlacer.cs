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
        await Utilities.RegisterPrefab("Prefab/Towers/BasicTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/BuffTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/IceTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/MissileTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/StrongTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/TankTower", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Towers/ThunderTower", prefabMap);

        //cam = GetComponentInChildren<Camera>();
        //if (cam != null)
        //{
        //    Debug.Log("Parent Camera found: " + cam.name);
        //}
        //else
        //{
        //    Debug.LogWarning("No Camera component found on the parent object.");
        //}
    }

    void Update()
    {
        if (TowerPlacementManager.instance.IsPlacementActive())
        {
            currentTowerPrefabId = TowerPlacementManager.instance.GetTowerPrefabId();
            currentCardId = TowerPlacementManager.instance.GetCardId();

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
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
        Protocol.C2G_TowerBuildRequest pkt = new Protocol.C2G_TowerBuildRequest
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

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_TowerBuildRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendPacket(sendBuffer);
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

        currentHighlight = Instantiate(isValidTile ? validTile : unvalidTile, worldPosition, Quaternion.identity);
    }

    bool CheckPlacement(Vector3Int cellPosition)
    {
        if (!tilemap.HasTile(cellPosition))
        {
            return false;
        }

        Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
        Collider2D[] hitcolliders = Physics2D.OverlapPointAll(tilemap.GetCellCenterWorld(cellPosition) + offset);
        foreach (var collider in hitcolliders)
        {
            Debug.Log($"Hit collider: {collider.name}, Tag: {collider.tag}");
            if (collider.CompareTag("Obstacle") || collider.CompareTag("Tower") || collider.CompareTag("Enemy") || collider.CompareTag("Player"))
            {
                return false;
            }
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