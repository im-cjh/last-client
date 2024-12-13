using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Protocol;
using Unity.Collections;
using System.Threading.Tasks;

public class SkillUser : TowerPlacer
{
    private string currentSkillPrefabId;
    public static new SkillUser instance = null;

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

        await Utilities.RegisterPrefab("Prefab/Skills/OrbitalBeam", prefabMap);
        await Utilities.RegisterPrefab("Prefab/Skills/TowerRepair", prefabMap);
    }

    void Update()
    {
        if (SkillManager.instance.IsSkillActiveOn())
        {
            currentSkillPrefabId = SkillManager.instance.GetSkillPrefabId();
            currentCardId = SkillManager.instance.GetCardId();

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            if (cellPosition != previousCellPosition)
            {
                HighlightTile(cellPosition);
                previousCellPosition = cellPosition;
            }

            if (Input.GetMouseButtonDown(0) && isValidTile) // 마우스 클릭
            {
                PlaceSkill(currentSkillPrefabId, currentCardId, cellPosition);
            }
        }
    }

    private void PlaceSkill(string prefabId, string cardId, Vector3Int cellPosition)
    {
        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
        worldPosition += offset;

        SendSkillRequestToServer(prefabId, cardId, worldPosition.x, worldPosition.y);

        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }
    }

    private void SendSkillRequestToServer(string prefabId, string uuid, float x, float y)
    {
        Debug.Log($"서버에게 스킬 사용 요청: prefabId:{prefabId}, x:{x}, y:{y}");
        Protocol.C2G_UseSkillRequest pkt = new Protocol.C2G_UseSkillRequest
        {
            Skill = new Protocol.SkillData
            {
                PrefabId = prefabId,
                SkillPos = new Protocol.PosInfo
                {
                    X = x,
                    Y = y
                }
            },
            RoomId = PlayerInfoManager.instance.roomId,
            CardId = uuid,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_UseSkillRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendPacket(sendBuffer);
    }

    public void UseSkill(SkillData skillData)
    {
        Debug.Log("UseSkill Called. skillData: " + skillData);
        Vector2 cellCenterWorld = new Vector2(skillData.SkillPos.X, skillData.SkillPos.Y);

        Instantiate(prefabMap[skillData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"스킬이 {cellCenterWorld} 위치에 사용되었습니다.");
        SkillManager.instance.SetSkillActive(false, null, null);
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

        Vector3 cellworldPosition = tilemap.GetCellCenterWorld(cellPosition);
        float distance = Vector3.Distance(transform.position, cellworldPosition);

        if (distance > maxPlacementDistance)
        {
            return false;
        }

        return true;
    }
}