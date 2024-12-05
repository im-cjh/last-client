using System;
using System.Collections;
using Unity.Mathematics;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using Protocol;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object")]
    public GameObject hud;
    public GameObject GameStartUI;

    [Header("# Managers")]
    private int initialHands = 7;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //hud.SetActive(true);
        //GameStartUI.SetActive(false);

        //AudioManager.instance.PlayBgm(true);
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);

        HandManager.instance.AddCard("BasicTower", "1234");
        // HandManager.instance.AddCard("OrbitalBeam", "123");
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        //AudioManager.instance.PlayBgm(true);
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void SendLocationUpdatePacket(float x, float y)
    {
        // Debug.Log("my pos: " + x + " , " + y);
        Protocol.C2B_PlayerPositionUpdateRequest pkt = new Protocol.C2B_PlayerPositionUpdateRequest
        {
            PosInfo = new Protocol.PosInfo
            {
                Uuid = PlayerInfoManager.instance.userId,
                X = x,
                Y = y
            },
            RoomId = PlayerInfoManager.instance.roomId
        };


        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_PositionUpdateRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }
}
