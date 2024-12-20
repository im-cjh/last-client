using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;

    private List<Vector3> usedPositions = new List<Vector3>();
    public static RandomObstacleSpawner instance;

    void Awake()
    {
        instance = this;
    }

    public void HandleSpawnObstacle(RepeatedField<Protocol.PosInfo> posInfos)
    {
        //Debug.Log("HandleSpawnObstacle Called");
        foreach (Protocol.PosInfo obstaclePos in posInfos)
        {
            //Vector3 cellPosition = new Vector3(obstaclePos.X+0.5f, obstaclePos.Y+0.5f, 0);
            Vector3 cellPosition = new Vector3(obstaclePos.X, obstaclePos.Y, 0);
            // 타일이 있는 위치인지 확인
            Instantiate(obstaclePrefab, cellPosition, Quaternion.identity);

            // 위치 기록
            usedPositions.Add(cellPosition);
        }
    }
}
