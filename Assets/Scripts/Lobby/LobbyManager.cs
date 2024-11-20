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
    
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void OnRecvRooms(List<RoomData> rooms)
    {
        uiMain.SetRoomList(rooms);
    }

    public void OnClickCreateRoom()
    {
        uiCreate.Opened();
    }

    public void OnEnteredRoom(RoomData roomData)
    {
        uiMain.gameObject.SetActive(false);
        uiRoom.gameObject.SetActive(true);
        uiRoom.SetRoomInfo(roomData);
    }

    public void OnJoinedRoomSomeone(Protocol.UserData userData)
    {

        uiRoom.AddUserToSlot(userData);
    }
}
