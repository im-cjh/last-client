using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 따라갈 대상 (캐릭터)
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // 카메라 오프셋
    [SerializeField] private float smoothSpeed = 0.125f; // 따라가는 속도

    private void LateUpdate()
    {
        // 목표 위치 계산
        Vector3 targetPosition = target.position + offset;

        // 부드럽게 이동 (SmoothDamp 또는 Lerp 사용 가능)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
