using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Public Fields
    public string nickname;          //// 캐릭터 닉네임
    public float speed = 5f;         // 이동 속도
    public bool isLocalPlayer;       // 로컬 플레이어 여부

    // Private Fields
    private Vector2 inputVec;        // 이동 방향
    private Rigidbody2D rigid;       // Rigidbody2D컴포넌트
    private SpriteRenderer spriteRenderer;  // SpriteRenderer 컴포넌트
    private TextMeshPro nicknameText;      // 닉네임 텍스트
    private Vector2 lastSyncedPosition; // 마지막으로 서버에 전송된 위치
    private Animator animator;

    // Constants
    private const float SyncThreshold = 0.1f; // ���� ����ȭ �ּ� �Ÿ�

    private void Awake()
    {
        // ������Ʈ �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        nicknameText = GetComponentInChildren<TextMeshPro>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // �г��� ǥ�� ����
        nicknameText.text = nickname;
        nicknameText.GetComponent<MeshRenderer>().sortingOrder = 6;
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            HandleInput();          // 로컬 플레이어만 입력 처리
            TrySendPositionToServer(); // 로컬 플레이어 위치 동기화
        }
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            MoveCharacter(); // 로컬 플레이어 이동 처리
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 입력 처리 (로컬 플레이어 전용)
    private void HandleInput()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        if (inputVec.magnitude > 0)
        {
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
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
}
