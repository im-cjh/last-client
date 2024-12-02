using Google.Protobuf.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomObstacleSpawner : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private int obstacleCount = 20;

    private List<Vector3> usedPositions = new List<Vector3>();
    public static RandomObstacleSpawner instance;

    void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    for (int i = 0; i < obstacleCount; i++)
    //    {
    //        // 랜덤 좌표 생성
    //        int x = Random.Range(-11, 10);
    //        int y = Random.Range(-14, 13);
    //        Vector3Int cellPosition = new Vector3Int(x, y, 0);

    //        // 타일이 있는 위치인지 확인
    //        if (tilemap.HasTile(cellPosition) && !usedPositions.Contains(cellPosition))
    //        {
    //            // Cell의 월드 좌표 계산
    //            Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);

    //            // 장애물 생성
    //            Instantiate(obstaclePrefab, worldPosition, Quaternion.identity);

    //            // 위치 기록
    //            usedPositions.Add(cellPosition);
    //        }
    //    }
    //}

    public void HandleSpawnObstacle(RepeatedField<Protocol.PosInfo> posInfos)
    {
        Debug.Log("HandleSpawnObstacle Called");
        foreach (Protocol.PosInfo obstaclePos in posInfos)
        {
            Vector3 cellPosition = new Vector3(obstaclePos.X+0.5f, obstaclePos.Y+0.5f, 0);
            // 타일이 있는 위치인지 확인
            Instantiate(obstaclePrefab, cellPosition, Quaternion.identity);

            // 위치 기록
            usedPositions.Add(cellPosition);
        }
    }
}
