using Protocol;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UICreateRoom: UIBase
{
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_Dropdown count;

    public override void Opened(object[] param = null)
    {
        Debug.Log("Opend Called");
        base.Opened(param);
        var roomNameSample = new List<string>() { "³Ê¸¸ ¿À¸é °í!", "°³³äÀÖ´Â »ç¶÷¸¸", "¾îµô ³ÑºÁ?", "Áñ°Å¿î °ÔÀÓ ÇÑÆÇ ÇÏ½¯?", "»§¾ß! »§¾ß!" };
        roomName.text = roomNameSample[Random.Range(0, roomNameSample.Count)];
    }

    public void RequestCreateRoom()
    {
        Debug.Log("RequestCreateRoom Called");
        Protocol.C2L_CreateRoomRequest pkt = new C2L_CreateRoomRequest();
        pkt.Name = roomName.text;
        pkt.MaxUserNum = count.value;

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2L_CreateRoom, 0);
        NetworkManager.instance.SendLobbyPacket(sendBuffer);

        this.Closed();
    }
}
