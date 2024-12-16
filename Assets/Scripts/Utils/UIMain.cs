using Protocol;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : UIListBase<ItemRoom>
{
    private float time = 0;
    private List<Protocol.RoomData> rooms;

    public override void Opened(object[] param)
    {
        // UI가 열렸을 때 초기화가 필요하면 구현
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > 5)
        {
            time = 0;
            OnRefreshRoomList();
        }
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }


    public void SetRoomList(List<RoomData> rooms)
    {
        this.rooms = rooms;
        SetList();
    }

    public void OnRefreshRoomList()
    {
        Protocol.C2G_GetRoomListRequest pkt = new Protocol.C2G_GetRoomListRequest();
        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_GetRoomListRequest, 0);

        Debug.Log("OnRefreshRoomList");
        NetworkManager.instance.SendPacket(sendBuffer);
    }

    public override void HideDirect()
    {
        //UIManager.Hide<UIMain>();
    }

    public override void SetList()
    {

        ClearList();
        for (int i = 0; i < rooms.Count; i++)
        {
            var item = AddItem();
            item.SetItem(rooms[i], OnClickJoinRoom);
        }
    }

    public void OnClickRandomMatch()
    {
        //if (SocketManager.instance.isConnected)
        //{
        //    GamePacket packet = new GamePacket();
        //    packet.JoinRandomRoomRequest = new C2SJoinRandomRoomRequest();
        //    SocketManager.instance.Send(packet);
        //}
    }


    public void OnClickJoinRoom(int roomId)
    {  
        Protocol.C2G_JoinRoomRequest pkt = new Protocol.C2G_JoinRoomRequest();
        pkt.RoomId = roomId;
        pkt.Nickname = PlayerInfoManager.instance.nickname;
        pkt.PrefabId = PlayerInfoManager.instance.prefabId;

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_JoinRoomRequest, 0);

        NetworkManager.instance.SendPacket(sendBuffer);
    }
}
