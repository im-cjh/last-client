using System;
using UnityEngine;

public class PlayerInfoManager : MonoBehaviour
{
    public static PlayerInfoManager instance;

    [Header("# Game Control")]
    public int targetFrameRate;
    public string version = "1.0.0";
    public int latency = 2;
    public int sequence = 0;

    [Header("# Player Info")]
    public string userId = "dd";
    public string nickname = "test";
    public Protocol.CharacterData characterData = new Protocol.CharacterData { CharacterType = Protocol.CharacterType.NoneCharacter};

    public Protocol.B2C_GameStartNotification tmp_gameStartPacket;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = targetFrameRate;
    }

    public int GetNextSequence()
    {
        return sequence++;
    }
}
