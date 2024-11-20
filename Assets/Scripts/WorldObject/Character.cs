using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    [SerializeField] private Rigidbody2D rigid;              // Rigidbody2D
    [SerializeField] private float speed = 5f;            // 이동 속도

    private Vector2 dir; // 이동 방향 저장

    private void Update()
    {
        // 이동 방향 입력 처리
        dir.x = Input.GetAxisRaw("Horizontal"); // A(-1) / D(+1)
        dir.y = Input.GetAxisRaw("Vertical");   // W(+1) / S(-1)

        // 방향 반전 처리 (가로 방향 입력이 있을 때만)
        if (dir.x > 0)
        {
            spriteRenderer.flipX = true; // 오른쪽을 바라봄
        }
        else if (dir.x < 0)
        {
            spriteRenderer.flipX = false; // 왼쪽을 바라봄
        }

        GameManager.instance.SendLocationUpdatePacket(rigid.position.x, rigid.position.y);
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 이용한 이동 처리
        rigid.MovePosition(rigid.position + dir * speed * Time.fixedDeltaTime);
    }
}
