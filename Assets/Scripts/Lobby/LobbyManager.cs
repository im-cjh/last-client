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
    public Button createRoomButton;            // 방 생성 버튼
    public Transform roomListContent;          // 방 목록 Scroll View의 Content
    public GameObject roomItemPrefab;          // RoomItem 프리팹
    public GameObject roomCreationPanel;       // 방 생성 패널
    public InputField roomNameInput;           // 방 이름 입력 필드
    public Button createRoomConfirmButton;     // 방 생성 확인 버튼
    public Button cancelRoomButton;            // 방 생성 취소 버튼

    private List<Protocol.RoomData> mRooms = new List<RoomData>(); // 방 목록 데이터 (예: 방 이름 및 인원 수)

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);

        // 초기 UI 설정 및 이벤트 연결
        createRoomButton.onClick.AddListener(ShowRoomCreationPanel);
        createRoomConfirmButton.onClick.AddListener(CreateRoom);
        cancelRoomButton.onClick.AddListener(HideRoomCreationPanel);

        // 서버로부터 방 목록 요청
        RequestRoomList();
    }

    // 방 생성 패널을 표시하는 함수
    private void ShowRoomCreationPanel()
    {
        roomNameInput.text = ""; // 입력 필드 초기화
        roomCreationPanel.SetActive(true); // 패널 활성화
    }

    // 방 생성 패널을 숨기는 함수
    private void HideRoomCreationPanel()
    {
        roomCreationPanel.SetActive(false); // 패널 비활성화
    }

    // 방 생성 함수
    private void CreateRoom()
    {
        //string roomName = roomNameInput.text.Trim();

        //// 방 이름이 비어있는지 확인
        //if (string.IsNullOrEmpty(roomName))
        //{
        //    Debug.Log("방 이름을 입력하세요.");
        //    return;
        //}

        //// 새 방 추가
        //mRooms.Add(new RoomData(1, roomName, 1, 4)); // 기본적으로 최대 인원 4명으로 설정

        //RefreshRoomList(); // 방 목록 갱신
        //HideRoomCreationPanel(); // 방 생성 패널 닫기
    }

    // 방 목록 UI 갱신 함수
    public void RefreshRoomList()
    {
        // 기존 방 목록 UI 제거
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 방 목록 UI 추가
        foreach (var room in mRooms)
        {
            GameObject roomItem = Instantiate(roomItemPrefab, roomListContent);

            // 방 이름 설정
            roomItem.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = room.Name;

            // 인원 수 설정 (예: "1/4")
            roomItem.transform.Find("RoomCountText").GetComponent<TextMeshProUGUI>().text = $"현재 인원 수/{room.MaxUserNum}";


            // 입장 버튼에 이벤트 추가
            Button enterButton = roomItem.transform.Find("EnterRoomButton").GetComponent<Button>();
            enterButton.onClick.AddListener(() => EnterRoom(room.Id));
        }
    }

    // 방 입장 함수
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
