using System;
using System.Collections;
using Unity.Mathematics;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object")]
    //public PoolManager pool;
    //public Player player;
    public GameObject hud;
    public GameObject GameStartUI;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //player.gameObject.SetActive(true);
        //hud.SetActive(true);
        //GameStartUI.SetActive(false);
        
        //AudioManager.instance.PlayBgm(true);
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
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
        Protocol.C2B_PositionUpdateRequest pkt = new Protocol.C2B_PositionUpdateRequest();
        pkt.EntityData = new Protocol.EntityData
        {
            ObjectType = Protocol.ObjectType.Player,
            Pos = new Protocol.PosInfo { X = x, Y = y },
            Uuid = PlayerInfoManager.instance.userId
        };
        pkt.RoomId = PlayerInfoManager.instance.roomId;

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_PositionUpdateRequest, PlayerInfoManager.instance.GetNextSequence());
        NetworkManager.instance.SendBattlePacket(sendBuffer);
    }

}
