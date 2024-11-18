using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // ���� ��� (ĳ����)
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10); // ī�޶� ������
    [SerializeField] private float smoothSpeed = 0.125f; // ���󰡴� �ӵ�

    private void LateUpdate()
    {
        // ��ǥ ��ġ ���
        Vector3 targetPosition = target.position + offset;

        // �ε巴�� �̵� (SmoothDamp �Ǵ� Lerp ��� ����)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
    }
}
