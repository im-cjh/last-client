using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using Protocol;
using UnityEditor.EditorTools;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    [Header("UI References")]
    public Button createRoomButton;            // �� ���� ��ư
    public Transform roomListContent;          // �� ��� Scroll View�� Content
    public GameObject roomItemPrefab;          // RoomItem ������
    public GameObject roomCreationPanel;       // �� ���� �г�
    public InputField roomNameInput;           // �� �̸� �Է� �ʵ�
    public Button createRoomConfirmButton;     // �� ���� Ȯ�� ��ư
    public Button cancelRoomButton;            // �� ���� ��� ��ư

    private List<Protocol.RoomData> mRooms = new List<RoomData>(); // �� ��� ������ (��: �� �̸� �� �ο� ��)

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);

        // �ʱ� UI ���� �� �̺�Ʈ ����
        createRoomButton.onClick.AddListener(ShowRoomCreationPanel);
        createRoomConfirmButton.onClick.AddListener(CreateRoom);
        cancelRoomButton.onClick.AddListener(HideRoomCreationPanel);

        // �����κ��� �� ��� ��û
        RequestRoomList();
    }

    // �� ���� �г��� ǥ���ϴ� �Լ�
    private void ShowRoomCreationPanel()
    {
        roomNameInput.text = ""; // �Է� �ʵ� �ʱ�ȭ
        roomCreationPanel.SetActive(true); // �г� Ȱ��ȭ
    }

    // �� ���� �г��� ����� �Լ�
    private void HideRoomCreationPanel()
    {
        roomCreationPanel.SetActive(false); // �г� ��Ȱ��ȭ
    }

    // �� ���� �Լ�
    private void CreateRoom()
    {
        //string roomName = roomNameInput.text.Trim();

        //// �� �̸��� ����ִ��� Ȯ��
        //if (string.IsNullOrEmpty(roomName))
        //{
        //    Debug.Log("�� �̸��� �Է��ϼ���.");
        //    return;
        //}

        //// �� �� �߰�
        //mRooms.Add(new RoomData(1, roomName, 1, 4)); // �⺻������ �ִ� �ο� 4������ ����

        //RefreshRoomList(); // �� ��� ����
        //HideRoomCreationPanel(); // �� ���� �г� �ݱ�
    }

    // �� ��� UI ���� �Լ�
    public void RefreshRoomList()
    {
        // ���� �� ��� UI ����
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // ���ο� �� ��� UI �߰�
        foreach (var room in mRooms)
        {
            GameObject roomItem = Instantiate(roomItemPrefab, roomListContent);

            // �� �̸� ����
            roomItem.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = room.Name;

            // �ο� �� ���� (��: "1/4")
            roomItem.transform.Find("RoomCountText").GetComponent<TextMeshProUGUI>().text = $"���� �ο� ��/{room.MaxUserNum}";


            // ���� ��ư�� �̺�Ʈ �߰�
            Button enterButton = roomItem.transform.Find("EnterRoomButton").GetComponent<Button>();
            enterButton.onClick.AddListener(() => EnterRoom(room.Id));
        }
    }

    // �� ���� �Լ�
    private void EnterRoom(int pRoomId)
    {
        Debug.Log("EnterRoom called");
    }

    public void OnRecvRooms(List<RoomData> pRoomData)
    {
        mRooms = pRoomData;

        RefreshRoomList();
    }

    public void OnRecvEnterRoomMe(List<UserData> pUserDatas, RoomData pRoomInfo)
    {
        Debug.Log("OnRecvEnterRoomMe");
    }

    public void RequestRoomList()
    {
    }
}
