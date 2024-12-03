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
    private string currentCardId;
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

        await RegisterPrefab("Prefab/Skills/OrbitalBeam");
        await RegisterPrefab("Prefab/Skills/TowerRepair");
    }

    void Update()
    {
        if (SkillManager.instance.IsSkillActiveOn())
        {
            currentSkillPrefabId = SkillManager.instance.GetSkillPrefabId();
            currentCardId = SkillManager.instance.GetCardId();

            HighlightTile();
            if (Input.GetMouseButtonDown(0)) // 마우스 클릭
            {
                PlaceSkill(currentSkillPrefabId, currentCardId);
                Destroy(currentHighlight);
            }
        }
    }

    private void PlaceSkill(string prefabId, string cardId)
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0; // z축 고정
        Debug.Log("mouseWorldPos: " + mouseWorldPos);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Tile"));

        if (hit.collider != null)
        {
            // collider와 충돌한 위치
            Vector3 hitPoint = hit.point;

            // 타일의 중심에 맞춤
            Vector3 snappedPosition = new Vector3(
                Mathf.Floor(hitPoint.x) + 0.5f,
                Mathf.Floor(hitPoint.y) + 0.5f,
                0
            );

            float distance = Vector2.Distance(transform.position, snappedPosition);
            if (distance > maxPlacementDistance)
            {
                Debug.Log("거리가 너무 멉니다.");
                return;
            }

            // Instantiate(prefabMap[prefabId], snappedPosition, Quaternion.identity);

            // 서버에 보내줄 타일의 위치
            Vector3 tilePosition = new Vector3(
                snappedPosition.x - 0.5f,
                snappedPosition.y - 0.5f,
                0
            );

            // Debug.Log($"{tilePosition}에 스킬 시전");
            SendSkillRequestToServer(prefabId, cardId, tilePosition.x, tilePosition.y);
        }
    }

    private void SendSkillRequestToServer(string prefabId, string uuid, float x, float y)
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
            RoomId = PlayerInfoManager.instance.roomId,
            CardId = uuid,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_SkillRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

    public void UseSkill(SkillData skillData)
    {
        Vector2 cellCenterWorld = new Vector2(skillData.SkillPos.X, skillData.SkillPos.Y);

        Instantiate(prefabMap[skillData.PrefabId], cellCenterWorld, Quaternion.identity);
        Debug.Log($"스킬이 {cellCenterWorld} 위치에 사용되었습니다.");
        SkillManager.instance.SetSkillActive(false, null, null);
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