using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // ��������Ʈ ������
    [SerializeField] private Rigidbody2D rigid;              // Rigidbody2D
    [SerializeField] private float speed = 5f;            // �̵� �ӵ�

    private Vector2 dir; // �̵� ���� ����

    private void Update()
    {
        // �̵� ���� �Է� ó��
        dir.x = Input.GetAxisRaw("Horizontal"); // A(-1) / D(+1)
        dir.y = Input.GetAxisRaw("Vertical");   // W(+1) / S(-1)

        // ���� ���� ó�� (���� ���� �Է��� ���� ����)
        if (dir.x > 0)
        {
            spriteRenderer.flipX = true; // �������� �ٶ�
        }
        else if (dir.x < 0)
        {
            spriteRenderer.flipX = false; // ������ �ٶ�
        }

        GameManager.instance.SendLocationUpdatePacket(rigid.position.x, rigid.position.y);
    }

    private void FixedUpdate()
    {
        // Rigidbody2D�� �̿��� �̵� ó��
        rigid.MovePosition(rigid.position + dir * speed * Time.fixedDeltaTime);
    }
}
