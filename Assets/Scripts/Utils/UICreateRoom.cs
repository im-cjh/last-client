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
        base.Opened(param);
        var roomNameSample = new List<string>() { "�ʸ� ���� ��!", "�����ִ� �����", "��� �Ѻ�?", "��ſ� ���� ���� �Ͻ�?", "����! ����!" };
        roomName.text = roomNameSample[Random.Range(0, roomNameSample.Count)];
    }

    public void RequestCreateRoom()
    {
        Protocol.C2L_CreateRoomRequest pkt = new C2L_CreateRoomRequest();
        pkt.Name = roomName.text;
        pkt.MaxUserNum = count.value+1; // 0���� �����ؼ� +1����� ��

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2L_CreateRoomRequest, 0);
        NetworkManager.instance.SendLobbyPacket(sendBuffer);

        this.Closed();
    }
}
