using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SpriteAnimation spriteAnimation; // Character의 SpriteAnimation
    [SerializeField] private Rigidbody2D characterRb; // Character의 Rigidbody2D
    [SerializeField] private float speed = 5f; // 이동 속도

    private Vector2 movement; // 이동 벡터

    private void Update()
    {
        // 이동 입력 처리
        movement.x = Input.GetAxisRaw("Horizontal"); // A, D 또는 화살표 좌우
        movement.y = Input.GetAxisRaw("Vertical");   // W, S 또는 화살표 상하

        // 상태 전환
        if (movement.sqrMagnitude > 0) // 입력 값이 있으면 Walk 애니메이션
        {
            spriteAnimation.SetState(eAnimationState.Walk);
        }
        else // 입력 값이 없으면 Idle 애니메이션
        {
            spriteAnimation.SetState(eAnimationState.Idle);
        }
    }

    private void FixedUpdate()
    {
        // Rigidbody2D를 사용해 이동 처리
        if (characterRb != null)
        {
            characterRb.MovePosition(characterRb.position + movement * speed * Time.fixedDeltaTime);
        }
    }
}
