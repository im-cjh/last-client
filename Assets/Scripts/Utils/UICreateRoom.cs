using Protocol;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UICreateRoom : UIBase
{
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private TMP_Dropdown count;

    public override void Opened(object[] param = null)
    {
        base.Opened(param);
        var roomNameSample = new List<string>() { "너만 오면 고!", "개념있는 사람만", "액션 쾌감!", "한판 ㄱㄱㄱ", "러다이트 운동하러가실분" };
        roomName.text = roomNameSample[Random.Range(0, roomNameSample.Count)];
    }

    public void RequestCreateRoom()
    {
        Debug.Log("ㅇㅇ");
        Protocol.C2G_CreateRoomRequest pkt = new C2G_CreateRoomRequest();
        pkt.Name = roomName.text;
        pkt.MaxUserNum = count.value+1; // 0부터 시작해서 +1해줘야 됨
        
        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_CreateRoomRequest, 0);
        NetworkManager.instance.SendPacket(sendBuffer);

        this.Closed();
    }
}
