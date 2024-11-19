using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float minX;

    [SerializeField]
    private float maxX;

    [SerializeField]
    private float minY;

    [SerializeField]
    private float maxY;

    void Start()
    {
        transform.position = player.transform.position;
    }

    void Update()
    {
        if (player != null){
            transform.position = player.transform.position + new Vector3(0, 0, -10f);
        }
    }
}
