using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using Protocol;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    [Header("UI References")]
    public UIMain uiMain;
    public UICreateRoom uiCreate;
    public UIRoom uiRoom;
    public UITutorial uiTutorial;
    

    void Awake()
    {
        instance = this;
    }

    public void OnRecvRooms(List<RoomData> rooms)
    {
        uiMain.SetRoomList(rooms);
    }

    public void OnClickCreateRoom()
    {
        uiCreate.Opened();
    }

    public void OnClickTutorial()
    {
        uiTutorial.gameObject.SetActive(true);
        // uiTutorial.Opened();
    }

    public void OnEnteredRoom(RoomData roomData)
    {
        uiMain.gameObject.SetActive(false);
        uiRoom.gameObject.SetActive(true);
        uiRoom.SetRoomInfo(roomData);

        PlayerInfoManager.instance.roomId = roomData.Id;
    }

    public void OnJoinedRoomSomeone(Protocol.UserData userData)
    {
        uiRoom.AddUserToSlot(userData);
    }

    public void onExitRoom()
    {
        uiMain.gameObject.SetActive(true);
        uiRoom.gameObject.SetActive(false);

        Protocol.C2G_LeaveRoomRequest pkt = new C2G_LeaveRoomRequest();
        pkt.RoomId = PlayerInfoManager.instance.roomId;
        if (PlayerInfoManager.instance.roomId == -1)
        {
            Debug.LogError("[onExitRoom] 유효하지 않은 roomId");
            return;
        }
        PlayerInfoManager.instance.roomId = -1;
        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_LeaveRoomRequest, 0);

        NetworkManager.instance.SendPacket(sendBuffer);
    }

    public void handleLobbyChat(G2C_ChatMessageNotification packet)
    {
        
    }
}
