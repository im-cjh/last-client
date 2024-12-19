using Protocol;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoom : UIBase
{
    [SerializeField] private List<ItemRoomSlot> slots;
    [SerializeField] private Button buttonExit;
    [SerializeField] private Button buttonStart;
    [SerializeField] private TMP_Text roomNo;
    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text roomCount;

    private List<UserData> users = new List<UserData>();
    private int maxUserCount;
    private Protocol.RoomData roomData;
    public LobbyChatManager roomChatManager;

    public override void Opened(object[] param)
    {
        // UI 숨기기
        //UIGnb.Instance?.Hide();

        // 방 정보 설정
        roomData = (RoomData)param[0];
        SetRoomInfo(roomData);
    }

    public void SetRoomInfo(RoomData pRoomData)
    {
        roomData = pRoomData;
        // 방 정보 UI 업데이트
        roomNo.text = roomData.Id.ToString();
        roomName.text = roomData.Name;
        maxUserCount = roomData.MaxUserNum;
        roomCount.text = $"{roomData.Users.Count}/{roomData.MaxUserNum}";

        // 사용자 리스트 업데이트
        users.Clear();
        users.AddRange(roomData.Users);

        for (int i = 0; i < slots.Count; i++)
        {
            var user = users.Count > i ? users[i] : null;
            slots[i].SetItem(user);
        }


        // 버튼 활성화 설정
        buttonStart.interactable = roomData.State == 0;
        buttonExit.interactable = roomData.State == 0;

        //방장이면 활성화, 근데 잠깐 주석처리함 ㅇㅇ [TODO]
        buttonStart.gameObject.SetActive(true);
        //buttonStart.gameObject.SetActive(roomData.OwnerId == PlayerInfoManager.instance.userId);
    }

    public void OnClickExit()
    {
        //HideDirect();
        LobbyManager.instance.onExitRoom();
    }

    public void OnClickGameStart()
    {
        RequestStartGame();
    }

    private void RequestStartGame()
    {
        // 게임 시작 로직
        Protocol.C2G_GameStartRequest pkt = new Protocol.C2G_GameStartRequest();
        pkt.RoomId = roomData.Id;
        //로비서버에서 방장인지 검증하기 위해서
        pkt.UserId = PlayerInfoManager.instance.userId;
        
        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_GameStartRequest, 0);
        NetworkManager.instance.SendPacket(sendBuffer);
    }

    public void HideDirect()
    {
        gameObject.SetActive(false); // UI 숨기기
    }

    public void AddUserToSlot(UserData user)
    {

        // 빈 슬롯 찾기
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                slots[i].SetItem(user); // 빈 슬롯에 유저 데이터 설정
                users.Add(user);       // 내부 리스트에 유저 추가
                UpdateRoomCount();     // 방 인원 수 갱신
                return;
            }
        }

        Debug.LogWarning("슬롯이 가득 찼습니다. 유저를 추가할 수 없습니다.");
    }

    public void RemoveUserFromSlot(string userId)
    {
        // 해당 유저를 슬롯에서 제거
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log("cnt");
            if (slots[i].HasUser(userId))
            {
                Debug.Log("true");
                slots[i].ClearItem();   // 슬롯 데이터 초기화
                users.RemoveAll(u => u.Id == userId); // 내부 리스트에서 유저 제거
                UpdateRoomCount();     // 방 인원 수 갱신
                return;
            }
        }

        Debug.LogWarning($"유저 {userId}를 슬롯에서 찾을 수 없습니다.");
    }
    private void UpdateRoomCount()
    {
        roomCount.text = $"{users.Count}/{maxUserCount}";
    }

    public string GetUserNicknameOrNull(string uuid)
    {
        foreach (var user in users)
        {
            if (user.Id== uuid)
            {
                return user.Name;
            }
        }

        return null;
    }

    public void onRecvLobbyChat(G2C_ChatMessageNotification packet)
    {
        string nickname = GetUserNicknameOrNull(packet.UserId);
        roomChatManager.AddMessageOnDisPlay(nickname, packet.Message);
    }
}
