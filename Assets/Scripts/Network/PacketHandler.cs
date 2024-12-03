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
        handlerMapping[ePacketID.B2C_PlayerPositionUpdateNotification] = HandleMove;
        handlerMapping[ePacketID.B2C_SpawnMonsterNotification] = HandleSpawnMonster;
        handlerMapping[ePacketID.B2C_MonsterDeathNotification] = HandleMonsterDeath;
        handlerMapping[ePacketID.B2C_MonsterPositionUpdateNotification] = HandleMonsterMove;

        handlerMapping[ePacketID.B2C_TowerBuildResponse] = HandleBuildTowerResponse;
        handlerMapping[ePacketID.B2C_TowerBuildNotification] = HandleBuildTowerNotification;
        handlerMapping[ePacketID.B2C_TowerAttackMonsterNotification] = HandleTowerAttackMonsterNotification;

        handlerMapping[ePacketID.B2C_UseSkillNotification] = HandleUseSkillNotification;
        handlerMapping[ePacketID.B2C_InitCardData] = HandleInitCardData;
    }

    /*---------------------------------------------
   [몬스터 이동 동기화]
---------------------------------------------*/
    static void HandleMonsterMove(byte[] pBuffer)
    {
        B2C_MonsterPositionUpdateNotification pkt = Protocol.B2C_MonsterPositionUpdateNotification.Parser.ParseFrom(pBuffer);

        EnemySpawner.instance.HandleMonsterMove(pkt.PosInfo);
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
        Protocol.L2C_JoinRoomNotification pkt = Protocol.L2C_JoinRoomNotification.Parser.ParseFrom(pBuffer);

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
        // 1. 패킷 파싱
        Protocol.B2C_GameStartNotification pkt = Protocol.B2C_GameStartNotification.Parser.ParseFrom(pBuffer);
        Debug.Log("게임 시작 패킷 수신");

        //temp
        // PlayerInfoManager.instance.tmp_obstaclePosInfos = pkt.ObstaclePosInfos;

        // 2. PlayerManager에 데이터 저장
        foreach (var playerData in pkt.PlayerDatas)
        {
            Debug.Log(playerData);
            PlayerManager.Instance.AddPlayer(playerData);
        }

        // 3. 게임 씬으로 전환
        SceneChanger.ChangeScene(SceneChanger.SceneType.TestGame);

        // 4. 씬 전환 후 캐릭터, 장애물 초기화 (중복 등록 방지)
        SceneChanger.OnSceneLoaded -= InitializeCharacters; // 기존 이벤트 제거
        SceneChanger.OnSceneLoaded += InitializeCharacters; // 새로운 이벤트 등록
    }

    // 캐릭터 초기화 메서드 (독립적인 메서드로 분리)
    private static void InitializeCharacters()
    {
        Debug.Log("게임 씬 로드 완료. 캐릭터 초기화 시작");
        CharacterManager.Instance.InitializeCharacters();

        //장애물도 일단 여기서 처리해주기...
        RandomObstacleSpawner.instance.HandleSpawnObstacle(PlayerInfoManager.instance.tmp_obstaclePosInfos);
    }



    /*---------------------------------------------
       [이동 동기화]
   ---------------------------------------------*/
    static void HandleMove(byte[] pBuffer)
    {
        try
        {
            // 1. 패킷 파싱
            Protocol.B2C_PlayerPositionUpdateNotification response = Protocol.B2C_PlayerPositionUpdateNotification.Parser.ParseFrom(pBuffer);

            // 2. 단일 위치 정보 처리
            var posInfo = response.PosInfo;
            // Debug.Log("HandleMove" + posInfo.X + ", " + posInfo.Y);
            // 3. 캐릭터 검색
            Character character = CharacterManager.Instance.GetCharacter(posInfo.Uuid);

            if (character != null)
            {
                // 로컬 플레이어가 아닌 경우에만 업데이트
                if (!character.isLocalPlayer)
                {
                    character.UpdatePositionFromServer(posInfo.X, posInfo.Y);
                }
            }
            else
            {
                Debug.LogWarning($"캐릭터를 찾을 수 없습니다: {posInfo.Uuid}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in HandleMove: {e.Message}");
        }
    }

    static void HandleSpawnMonster(byte[] pBuffer)
    {
        B2C_SpawnMonsterNotification packet = Protocol.B2C_SpawnMonsterNotification.Parser.ParseFrom(pBuffer);

        EnemySpawner.instance.SpawnMonster(packet.PrefabId, packet.PosInfo);
    }

    static void HandleMonsterDeath(byte[] pBuffer)
    {
        Debug.Log("HandleMonsterDeath Called");

        B2C_MonsterDeathNotification packet = Protocol.B2C_MonsterDeathNotification.Parser.ParseFrom(pBuffer);
        ScoreManager.instance.AddScore();
    }

    static void HandleBuildTowerResponse(byte[] pBuffer)
    {
        Debug.Log("HandleBuildTowerResponse Called");

        B2C_TowerBuildResponse packet = Protocol.B2C_TowerBuildResponse.Parser.ParseFrom(pBuffer);
        if (!packet.IsSuccess)
        {
            Debug.Log("타워 설치에 실패하였습니다.");
        }
    }

    static void HandleBuildTowerNotification(byte[] pBuffer)
    {
        Debug.Log("HandleBuildTowerNotification Called");

        B2C_TowerBuildNotification packet = Protocol.B2C_TowerBuildNotification.Parser.ParseFrom(pBuffer);

        TowerPlacer.instance.BuildTower(packet.Tower);
    }

    static void HandleTowerAttackMonsterNotification(byte[] pBuffer)
    {
        Debug.Log("HandleTowerAttackMonsterNotification");

        B2C_TowerAttackMonsterNotification packet = Protocol.B2C_TowerAttackMonsterNotification.Parser.ParseFrom(pBuffer);

        Tower tower = TowerManager.instance.GetTowerByUuid(packet.TowerId);
        if (tower != null)
        {
            tower.AttackTarget(packet.MonsterPos);
        }
    }

    static void HandleUseSkillNotification(byte[] pBuffer)
    {
        Debug.Log("HandleUseSkillNotification Called");

        B2C_UseSkillNotification packet = Protocol.B2C_UseSkillNotification.Parser.ParseFrom(pBuffer);

        Debug.Log("HandleUseSkillNotification packet: " + packet);

        SkillUser.instance.UseSkill(packet.Skill);
    }

    static void HandleInitCardData(byte[] pBuffer)
    {
        Debug.Log("HandleInitCardData Called");

        B2C_InitCardData packet = Protocol.B2C_InitCardData.Parser.ParseFrom(pBuffer);

        HandManager.instance.AddInitCard(packet.CardData);
    }
}

