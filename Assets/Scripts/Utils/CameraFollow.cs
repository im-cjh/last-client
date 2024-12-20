using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    public static CameraFollow instance;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position + new Vector3(0, 0, -10f);
            //Debug.Log($"카메라 위치 업데이트. 플레이어 위치: {player.transform.position}");
        }
        else
        {
            // Debug.LogWarning("CameraFollow: Player가 null입니다.");
        }
    }


    public void SetPlayer(GameObject pPlayer)
    {
        if (pPlayer == null)
        {
            Debug.LogError("SetPlayer: 전달받은 플레이어가 null입니다.");
            return;
        }

        player = pPlayer;
        transform.position = player.transform.position;
        Debug.Log($"SetPlayer 호출됨. 플레이어 이름: {player.name}, 위치: {player.transform.position}");
    }
}
