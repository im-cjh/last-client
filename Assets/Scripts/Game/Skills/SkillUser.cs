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
            Debug.Log("SkillUser instance");
            instance = this;
        }
    }

    async void Start()
    {
        tilemap = Utilities.FindAndAssign<Tilemap>("Grid/Tile");

        await RegisterPrefab("Prefab/Skills/OrbitBeam");
        await RegisterPrefab("Prefab/Skills/TowerRepair");
    }

    public void SetSkillPrefabId(string skillPrefabId)
    {
        currentSkillPrefabId = skillPrefabId;
    }

    void Update()
    {
        if (SkillManager.instance.IsSkillActiveOn())
        {
            currentSkillPrefabId = SkillManager.instance.GetSkillPrefabId();
            HighlightTile();
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭
            {
                PlaceSkill(currentSkillPrefabId);
                Destroy(currentHighlight);
            }
        }
    }

    private void PlaceSkill(string prefabId)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일맵의 Cell 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

        // cellPosition.x -= 15;
        // cellPosition.y -= 15;
        // 클릭한 셀에 타일이 있는지 확인
        if (tilemap.HasTile(cellPosition))
        {
            // 셀의 중심 월드 좌표 계산
            Vector3 cellCenterWorld = tilemap.GetCellCenterWorld(cellPosition);

            // 캐릭터와의 거리 계산
            float distance = Vector3.Distance(transform.position, cellCenterWorld);
            if (distance > maxPlacementDistance)
            {
                Debug.Log("거리가 너무 멉니다.");
                return;
            }

            // 최대 거리 안이면 스킬 사용 요청 보냄
            SendSkillRequestToServer(prefabId, cellPosition.x, cellPosition.y);
        }
    }

    private void SendSkillRequestToServer(string prefabId, float x, float y)
    {
        Debug.Log($"서버에게 스킬 사용 요청: prefabId:{prefabId}, x:{x}, y:{y}");
        Protocol.C2B_SkillRequest pkt = new Protocol.C2B_SkillRequest
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
            OwnerId = PlayerInfoManager.instance.userId,
            RoomId = PlayerInfoManager.instance.roomId,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_SkillRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

    public void UseSkill(SkillData skillData)
    {
        Vector2 cellCenterWorld = new Vector2(skillData.SkillPos.X, skillData.SkillPos.Y);

        Instantiate(prefabMap[skillData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"스킬이 {cellCenterWorld} 위치에 사용되었습니다.");
        TowerPlacementManager.instance.SetPlacementState(false, "");
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

                float distance = Vector3.Distance(transform.position, cellCenterWorld);

                // 설치 가능 여부에 따라 적절한 하이라이트 생성
                if (distance <= maxPlacementDistance)
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