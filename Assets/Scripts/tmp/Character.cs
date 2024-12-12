using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine.Animations;

public class Character : MonoBehaviour
{
    // Public Fields
    public string nickname;          //// 캐릭터 닉네임
    public float speed = 5f;         // 이동 속도
    public bool isLocalPlayer;       // 로컬 플레이어 여부
    public bool isTowerActive;
    public bool isSkillActive;

    // Private Fields
    private Vector2 inputVec;        // 이동 방향
    private Rigidbody2D rigid;       // Rigidbody2D컴포넌트
    private TextMeshPro nicknameText;      // 닉네임 텍스트
    private Vector2 lastSyncedPosition; // 마지막으로 서버에 전송된 위치
    private Animator animator;
    private string characterId;

    [SerializeField] private GameObject validTile; // 설치 가능한 타일 색상
    [SerializeField] private GameObject unvalidTile; // 설치 불가능한 타일 색상
    private bool isValidTile;
    private GameObject currentHighlight;
    private Vector3Int previousCellPosition = Vector3Int.zero;
    private Tilemap tilemap;
    private float maxPlacementDistance = 5f;

    private string cardPrefabId;
    private string cardId;
    //public Camera cam;

    // Constants
    private const float SyncThreshold = 0.1f; // ���� ����ȭ �ּ� �Ÿ�

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        nicknameText = GetComponentInChildren<TextMeshPro>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        tilemap = Utilities.FindAndAssign<Tilemap>("Grid/Tile");
    }

    private void OnEnable()
    {
        nicknameText.text = nickname;
        nicknameText.GetComponent<MeshRenderer>().sortingOrder = 6;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();          // 로컬 플레이어만 입력 처리
            TrySendPositionToServer(); // 로컬 플레이어 위치 동기화

            if (isTowerActive)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

                if (cellPosition != previousCellPosition)
                {
                    HighlightTile(cellPosition, true);
                    previousCellPosition = cellPosition;
                }

                if (Input.GetMouseButtonDown(0) && isValidTile) // 마우스 클릭
                {
                    Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
                    Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
                    worldPosition += offset;
                    SendBuildRequestToServer(worldPosition.x, worldPosition.y);

                    Destroy(currentHighlight);
                }
            }

            if (isSkillActive)
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

                if (cellPosition != previousCellPosition)
                {
                    HighlightTile(cellPosition, false);
                    previousCellPosition = cellPosition;
                }

                if (Input.GetMouseButtonDown(0) && isValidTile) // 마우스 클릭
                {
                    Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
                    Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
                    worldPosition += offset;
                    SendSkillRequestToServer(worldPosition.x, worldPosition.y);

                    Destroy(currentHighlight);
                }
            }
        }
    }

    private void HighlightTile(Vector3Int cellPosition, bool isTower)
    {
        // 기존 하이라이트 제거
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
        }

        // 유효한 타일인지 검사
        isValidTile = CheckPlacement(cellPosition, isTower);

        // 적절한 하이라이트 생성
        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
        Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
        worldPosition += offset;

        currentHighlight = Instantiate(isValidTile ? validTile : unvalidTile, worldPosition, Quaternion.identity);
    }

    private bool CheckPlacement(Vector3Int cellPosition, bool isTower)
    {
        if (!tilemap.HasTile(cellPosition))
        {
            return false;
        }

        if (isTower)
        {
            Vector3 offset = new Vector3(tilemap.cellSize.x * 0.5f, tilemap.cellSize.y * 0.5f, 0);
            Collider2D[] hitcolliders = Physics2D.OverlapPointAll(tilemap.GetCellCenterWorld(cellPosition) + offset);
            foreach (var collider in hitcolliders)
            {
                if (collider.CompareTag("Obstacle") || collider.CompareTag("Tower") || collider.CompareTag("Enemy") || collider.CompareTag("Player"))
                {
                    return false;
                }
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

    public void SetPrefabId(string prefabId, string uuid)
    {
        cardPrefabId = prefabId;
        cardId = uuid;
    }

    private void SendBuildRequestToServer(float x, float y)
    {
        // tower의 uuid는 서버에서 만들어서 보내줌
        Debug.Log($"서버에게 타워 설치 요청: prefabId:{cardPrefabId}, x:{x}, y:{y}");
        Protocol.C2B_TowerBuildRequest pkt = new Protocol.C2B_TowerBuildRequest
        {
            Tower = new Protocol.TowerData
            {
                TowerPos = new Protocol.PosInfo
                {
                    X = x,
                    Y = y,
                },
                PrefabId = cardPrefabId
            },
            RoomId = PlayerInfoManager.instance.roomId,
            OwnerId = PlayerInfoManager.instance.userId,
            CardId = cardId,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_TowerBuildRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

    private void SendSkillRequestToServer(float x, float y)
    {
        Debug.Log($"서버에게 스킬 사용 요청: prefabId:{cardPrefabId}, x:{x}, y:{y}");
        Protocol.C2B_SkillRequest pkt = new Protocol.C2B_SkillRequest
        {
            Skill = new Protocol.SkillData
            {
                PrefabId = cardPrefabId,
                SkillPos = new Protocol.PosInfo
                {
                    X = x,
                    Y = y
                }
            },
            RoomId = PlayerInfoManager.instance.roomId,
            CardId = cardId,
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_SkillRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            MoveCharacter(); // 로컬 플레이어 이동 처리
        }
    }

    // 입력 처리 (로컬 플레이어 전용)
    private void HandleInput()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        bool isWalking = inputVec.magnitude > 0;
        animator.SetBool("isWalk", isWalking);

        if (isWalking != animator.GetBool("isWalk"))
        {
            // GameManager.instance.SendAnimationUpdatePacket("isWalk", isWalking);
        }

        Vector3 curScale = transform.localScale;
        Vector3 curTextScale = nicknameText.transform.localScale;
        if (inputVec.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(curScale.x), curScale.y, curScale.z);
            nicknameText.transform.localScale = new Vector3(-Mathf.Abs(curTextScale.x), curTextScale.y, curTextScale.z);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Abs(curScale.x), curScale.y, curScale.z);
            nicknameText.transform.localScale = new Vector3(Mathf.Abs(curTextScale.x), curTextScale.y, curTextScale.z);
        }
    }

    // 캐릭터 이동 처리
    private void MoveCharacter()
    {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    // 서버로 위치 동기화 (로컬 플레이어 전용)
    private void TrySendPositionToServer()
    {
        if (Vector2.Distance(lastSyncedPosition, rigid.position) > SyncThreshold)
        {
            GameManager.instance.SendLocationUpdatePacket(rigid.position.x, rigid.position.y);
            lastSyncedPosition = rigid.position;
        }
    }

    // 서버로부터 받은 위치 데이터로 캐릭터 위치 업데이트
    public void UpdatePositionFromServer(float x, float y)
    {
        if (!isLocalPlayer) // 로컬 플레이어는 서버에서 받은 위치를 적용하지 않음
        {
            Vector2 serverPosition = new Vector2(x, y);
            //rigid.MovePosition(Vector2.Lerp(rigid.position, serverPosition, 0.1f)); // 부드럽게 위치 보간
            rigid.MovePosition(serverPosition); // �����̵�
        }
    }

    public void UpdateAnimationFromServer(string parameter, bool state)
    {
        if (!isLocalPlayer)
        {
            animator.SetBool(parameter, state);
        }
    }

    public void SetCharacterId(string uuid)
    {
        characterId = uuid;
    }

    public string GetCharacterId()
    {
        return characterId;
    }
}
