using TMPro;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Public Fields
    public string nickname;          // ĳ���� �г���
    public float speed = 5f;         // �̵� �ӵ�
    public bool isLocalPlayer;       // ���� �÷��̾� ����

    // Private Fields
    private Vector2 inputVec;        // �̵� ����
    private Rigidbody2D rigid;       // Rigidbody2D ������Ʈ
    private SpriteRenderer spriteRenderer;  // SpriteRenderer ������Ʈ
    private TextMeshPro nicknameText;      // �г��� �ؽ�Ʈ
    private Vector2 lastSyncedPosition; // ���������� ������ ���۵� ��ġ
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
            HandleInput();          // ���� �÷��̾ �Է� ó��
            TrySendPositionToServer(); // ���� �÷��̾� ��ġ ����ȭ
        }

        UpdateSpriteDirection(); // ��� ĳ������ ���� ������Ʈ
    }

    private void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            MoveCharacter(); // ���� �÷��̾� �̵� ó��
        }
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    /// �Է� ó�� (���� �÷��̾� ����)
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

        if (inputVec.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (inputVec.x < 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    /// ĳ���� �̵� ó��
    private void MoveCharacter()
    {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    /// ĳ���� ���� ������Ʈ
    private void UpdateSpriteDirection()
    {
        Vector3 curScale = transform.localScale;
        if (inputVec.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(curScale.x), curScale.y, curScale.z);
        }
        else if (inputVec.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(curScale.x), curScale.y, curScale.z);
        }
    }

    /// ������ ��ġ ����ȭ (���� �÷��̾� ����)
    private void TrySendPositionToServer()
    {
        if (Vector2.Distance(lastSyncedPosition, rigid.position) > SyncThreshold)
        {
            GameManager.instance.SendLocationUpdatePacket(rigid.position.x, rigid.position.y);
            lastSyncedPosition = rigid.position;
        }
    }

    /// �����κ��� ���� ��ġ �����ͷ� ĳ���� ��ġ ������Ʈ
    public void UpdatePositionFromServer(float x, float y)
    {
        if (!isLocalPlayer) // ���� �÷��̾�� �������� ���� ��ġ�� �������� ����
        {
            Vector2 serverPosition = new Vector2(x, y);
            //rigid.MovePosition(Vector2.Lerp(rigid.position, serverPosition, 0.1f)); // �ε巴�� ��ġ ����
            rigid.MovePosition(serverPosition); // �����̵�
        }
    }
}
