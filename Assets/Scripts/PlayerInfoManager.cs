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
    public int playerId = 1;
    public string nickname = "test";
    public int roomId;

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
