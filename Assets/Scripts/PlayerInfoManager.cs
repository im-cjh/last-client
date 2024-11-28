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
    public string userId = "ddd";
    public string nickname = "test";
    public int roomId = 0;
    public string prefabId = "Red";

    public Protocol.B2C_GameStartNotification tmp_gameStartPacket;

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
}
