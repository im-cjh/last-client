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

    public void SetRoomList(List<RoomData> rooms)
    {
        this.rooms = rooms;
        SetList();
    }

    public void OnRefreshRoomList()
    {
        //if (SocketManager.instance.isConnected)
        //{
        //    GamePacket packet = new GamePacket();
        //    packet.GetRoomListRequest = new C2SGetRoomListRequest();
        //    SocketManager.instance.Send(packet);
        //}
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
            item.SetItem(rooms[i], OnJoinRoom);
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

    public void OnClickCreateRoom()
    {
        //UIManager.Show<PopupRoomCreate>();
    }

    public void OnJoinRoom(int idx)
    {
        //if (SocketManager.instance.isConnected)
        //{
        //    GamePacket packet = new GamePacket();
        //    packet.JoinRoomRequest = new C2SJoinRoomRequest() { RoomId = idx };
        //    SocketManager.instance.Send(packet);
        //}
    }
}
