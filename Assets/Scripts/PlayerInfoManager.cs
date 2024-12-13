using Google.Protobuf.Collections;
using System;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager instance;

    [Header("# Game Control")]
    public int targetFrameRate;
    public string version = "1.0.0";
    public int latency = 200;
    public int sequence = 0;

    [Header("# Player Info")]
    public string userId;
    public string nickname = "test";
    public int roomId = -1;
    public string prefabId = "Red";

    public RepeatedField<Protocol.PosInfo> tmp_obstaclePosInfos;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = targetFrameRate;
    }

    private void Start()
    {
        //TODO 임시로 작성, 추후에 로그인 서버로부터 받아야 함 
        userId = Guid.NewGuid().ToString();
    }

    public int GetNextSequence()
    {
        return sequence++;
    }

    public void SetPrefabId(string characterPrefabId)
    {
        prefabId = characterPrefabId;
    }
}
