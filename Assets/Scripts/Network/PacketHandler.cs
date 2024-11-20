using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/*---------------------------------------------
    [패킷 핸들러]

    - 목적: ePacketID를 넘겨주면 대응되는 함수를 넘겨주기 위함

        [주의사항]
            - 에러 발생 시, 호출 쪽에서 처리할 것
---------------------------------------------*/
public class PacketHandler
{
    /*---------------------------------------------
        [멤버 변수]

        -Action은 반환 타입이 없는 delegate
---------------------------------------------*/
    public static Dictionary<ePacketID, Action<byte[]>> handlerMapping;

    /*---------------------------------------------
    [생성자]
---------------------------------------------*/
    static PacketHandler()
    {
        handlerMapping = new Dictionary<ePacketID, Action<byte[]>>();
        Init();
    }

    /*---------------------------------------------
    [초기화]
---------------------------------------------*/
    static void Init()
    {
        handlerMapping[ePacketID.L2C_Init] = HandleInitPacket;
        handlerMapping[ePacketID.L2C_GetRoomListResponse] = HandleRoomsPacket;
        handlerMapping[ePacketID.L2C_JoinRoomResponse] = HandleJoinRoomResponsePacket;
        handlerMapping[ePacketID.L2C_JoinRoomNotification] = HandleJoinRoomNotificationPacket;
        handlerMapping[ePacketID.L2C_CreateRoomResponse] = HandleCreateRoomResponsePacket;

        handlerMapping[ePacketID.L2C_GameStart] = HandleLobbyGameStart;
        handlerMapping[ePacketID.B2C_GameStartNotification] = HandleBattleGameStart;
        handlerMapping[ePacketID.B2C_PositionUpdateNotification] = HandleMove;
        //handlerMapping[ePacketID.B2C_Enter] = HandleEnterGame;
    }

    static void HandleInitPacket(byte[] pBuffer)
    {
        //패킷 역직렬화
        Protocol.L2C_Init pkt = Protocol.L2C_Init.Parser.ParseFrom(pBuffer);
        //임시
       // SceneChanger.ChangeLobbyScene();
        //GameManager.instance.GameStart();
    }

    static void HandleRoomsPacket(byte[] pBuffer)
    {
        //패킷 역직렬화
        Protocol.L2C_GetRoomListResponse pkt = Protocol.L2C_GetRoomListResponse.Parser.ParseFrom(pBuffer);

        List<RoomData> rooms = new List<RoomData>();

        foreach (var room in pkt.Rooms)
        {
            rooms.Add(room);
        }

        LobbyManager.instance.OnRecvRooms(rooms);
    }

    static void HandleJoinRoomResponsePacket(byte[] pBuffer)
    {
        Debug.Log("HandleJoinRoomResponsePacket");
        Protocol.L2C_JoinRoomResponse pkt = Protocol.L2C_JoinRoomResponse.Parser.ParseFrom(pBuffer);
        LobbyManager.instance.OnEnteredRoom(pkt.RoomInfo);
        
    }
    static void HandleJoinRoomNotificationPacket(byte[] pBuffer)
    {
        //패킷 역직렬화
        Protocol.L2C_JoinRoomNotification pkt  = Protocol.L2C_JoinRoomNotification.Parser.ParseFrom(pBuffer);
        
        LobbyManager.instance.OnJoinedRoomSomeone(pkt.JoinUser);
    }

    /*---------------------------------------------
    
    [방 생성]
---------------------------------------------*/
    static void HandleCreateRoomResponsePacket(byte[] pBuffer)
    {
        //패킷 역직렬화
        Protocol.L2C_CreateRoomResponse pkt = Protocol.L2C_CreateRoomResponse.Parser.ParseFrom(pBuffer);
        
        //방 입장 요청 보내기
        LobbyManager.instance.uiMain.OnClickJoinRoom(pkt.Room.Id);
    }

    /*---------------------------------------------
    [게임 시작 1/2]
    - 배틀 서버의 주소와 포트번호, 방 ID를 받아옴
    - 배틀 서버에 연결
---------------------------------------------*/
    static void HandleLobbyGameStart(byte[] pBuffer)
    {
        Protocol.L2C_GameStart pkt = Protocol.L2C_GameStart.Parser.ParseFrom(pBuffer);

        Debug.Log("게임시작 1");
        PlayerInfoManager.instance.roomId = pkt.RoomId;
        NetworkManager.instance.ConnectToBattleServer(pkt.Host, pkt.Port, pkt.RoomId);
    }

    /*---------------------------------------------
    [게임 시작 2/2]
    - 모든 플레이어가 접속하여 게임 시작
---------------------------------------------*/
    static void HandleBattleGameStart(byte[] pBuffer)
    {
        Debug.Log("끄어어억");
        Protocol.B2C_GameStartNotification pkt = Protocol.B2C_GameStartNotification.Parser.ParseFrom(pBuffer);
        PlayerInfoManager.instance.tmp_gameStartPacket = pkt;
        
        SceneChanger.ChangeGameScene();
    }

/*---------------------------------------------
    [이동 동기화]
---------------------------------------------*/
    static void HandleMove(byte[] pBuffer)
    {
        Debug.Log("HandleMove");
        try
        {
            Protocol.B2C_PositionUpdateNotification response = Protocol.B2C_PositionUpdateNotification.Parser.ParseFrom(pBuffer);

            //Spawner.instance.Spawn(response);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error HandleLocationPacket: {e.Message}");
        }
    }
}

