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

    public override void Opened(object[] param)
    {
        // UI �����
        //UIGnb.Instance?.Hide();

        // �� ���� ����
        roomData = (RoomData)param[0];
        SetRoomInfo(roomData);
    }

    public void SetRoomInfo(RoomData pRoomData)
    {
        Debug.Log("SetRoomInfo: " + pRoomData);
        Debug.Log("SetRoomInfo: " + pRoomData.Users);
        roomData = pRoomData;
        // �� ���� UI ������Ʈ
        roomNo.text = roomData.Id.ToString();
        roomName.text = roomData.Name;
        maxUserCount = roomData.MaxUserNum;
        roomCount.text = $"{roomData.Users.Count}/{roomData.MaxUserNum}";

        // ����� ����Ʈ ������Ʈ
        users.Clear();
        users.AddRange(roomData.Users);

        for (int i = 0; i < slots.Count; i++)
        {
            var user = users.Count > i ? users[i] : null;
            slots[i].SetItem(user);
        }


        // ��ư Ȱ��ȭ ����
        buttonStart.interactable = roomData.State == 0;
        buttonExit.interactable = roomData.State == 0;

        //�����̸� Ȱ��ȭ, �ٵ� ��� �ּ�ó���� ���� [TODO]
        buttonStart.gameObject.SetActive(true);
        //buttonStart.gameObject.SetActive(roomData.OwnerId == PlayerInfoManager.instance.userId);
    }

    public void OnClickExit()
    {
        HideDirect();
    }

    public void OnClickGameStart()
    {
        RequestStartGame();
    }

    private void RequestStartGame()
    {
        Debug.Log("RequestStartGame");
        Debug.Log(roomData);
        // ���� ���� ����
        Protocol.C2L_GameStart pkt = new Protocol.C2L_GameStart();
        pkt.RoomId = roomData.Id;
        //�κ񼭹����� �������� �����ϱ� ���ؼ�
        pkt.UserId = PlayerInfoManager.instance.userId;

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2L_GameStart, 0);
        NetworkManager.instance.SendLobbyPacket(sendBuffer);
    }

    public void HideDirect()
    {
        gameObject.SetActive(false); // UI �����
    }

    public void AddUserToSlot(UserData user)
    {
        Debug.Log("AddUserToSlot" + user.Name);
        Debug.Log("AddUserToSlot" + slots.Count);

        // �� ���� ã��
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log("AddUserToSlot" + user.Name);
            if (slots[i].IsEmpty())
            {
                Debug.Log("true");
                slots[i].SetItem(user); // �� ���Կ� ���� ������ ����
                users.Add(user);       // ���� ����Ʈ�� ���� �߰�
                UpdateRoomCount();     // �� �ο� �� ����
                return;
            }
        }

        Debug.LogWarning("������ ���� á���ϴ�. ������ �߰��� �� �����ϴ�.");
    }

    public void RemoveUserFromSlot(string userId)
    {
        // �ش� ������ ���Կ��� ����
        for (int i = 0; i < slots.Count; i++)
        {
            Debug.Log("cnt");
            if (slots[i].HasUser(userId))
            {
                Debug.Log("true");
                slots[i].ClearItem();   // ���� ������ �ʱ�ȭ
                users.RemoveAll(u => u.Id == userId); // ���� ����Ʈ���� ���� ����
                UpdateRoomCount();     // �� �ο� �� ����
                return;
            }
        }

        Debug.LogWarning($"���� {userId}�� ���Կ��� ã�� �� �����ϴ�.");
    }
    private void UpdateRoomCount()
    {
        roomCount.text = $"{users.Count}/{maxUserCount}";
    }


}
