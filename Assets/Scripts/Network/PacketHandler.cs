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
        //100번
        handlerMapping[ePacketID.G2C_CreateRoomResponse] = HandleCreateRoomResponsePacket;
        handlerMapping[ePacketID.G2C_CreateGameRoomNotification] = HandleCreateGameRoomNotification;
        handlerMapping[ePacketID.G2C_GameStartNotification] = HandleBattleGameStart;
        handlerMapping[ePacketID.G2C_GetRoomListResponse] = HandleRoomsPacket;
        handlerMapping[ePacketID.G2C_JoinRoomResponse] = HandleJoinRoomResponsePacket;
        handlerMapping[ePacketID.G2C_JoinRoomNotification] = HandleJoinRoomNotificationPacket;
        handlerMapping[ePacketID.G2C_LeaveRoomNotification] = HandleLeaveRoomNotificationPacket;
        handlerMapping[ePacketID.G2C_ChatMessageNotification] = HandleChatMessageNotification;


        //200번
        handlerMapping[ePacketID.G2C_SpawnMonsterNotification] = HandleSpawnMonster;
        handlerMapping[ePacketID.G2C_MonsterPositionUpdateNotification] = HandleMonsterMove;
        handlerMapping[ePacketID.G2C_MonsterAttackTowerNotification] = HandleMonsterAttackTower;
        handlerMapping[ePacketID.G2C_MonsterAttackBaseNotification] = HandleMonsterAttackBase;
        handlerMapping[ePacketID.G2C_MonsterDeathNotification] = HandleMonsterDeath;
        handlerMapping[ePacketID.G2C_MonsterHealthUpdateNotification] = HandleMonsterHealthUpdateNotification;
        handlerMapping[ePacketID.G2C_IncreaseWaveNotification] = HandleIncreaseWaveNotification;


        //300번
        //handlerMapping[ePacketID.B2C_TowerBuildResponse] = HandleBuildTowerResponse;
        handlerMapping[ePacketID.G2C_TowerBuildNotification] = HandleBuildTowerNotification;
        handlerMapping[ePacketID.G2C_TowerAttackMonsterNotification] = HandleTowerAttackMonsterNotification;
        handlerMapping[ePacketID.G2C_TowerDestroyNotification] = HandleTowerDestroyNotification;
        handlerMapping[ePacketID.G2C_TowerHealthUpdateNotification] = HandleTowerHealthUpdateNotification;
        handlerMapping[ePacketID.G2C_BaseDestroyNotification] = HandleBaseDestroyNotification;


        //400번
        handlerMapping[ePacketID.G2C_UseSkillResponse] = HandleSkillResponse;
        handlerMapping[ePacketID.G2C_UseSkillNotification] = HandleUseSkillNotification;

        //500번
        handlerMapping[ePacketID.G2C_InitCardData] = HandleInitCardData;
        handlerMapping[ePacketID.G2C_PlayerPositionUpdateNotification] = HandleMove;
        handlerMapping[ePacketID.G2C_PlayerUseAbilityNotification] = HandlePlayerUseAbilityNotification;
        handlerMapping[ePacketID.G2C_TowerBuffNotification] = HandleTowerBuffNotification;
        handlerMapping[ePacketID.G2C_AddCard] = HandleAddCard;
    }

    private static void HandleBaseDestroyNotification(byte[] pBuffer)
    {
        Debug.Log("게임 종료");
        // SceneChanger.ChangeScene(SceneChanger.SceneType.Lobby);
        //GameOver.isGameOver = true;
        GameOver.instance.ShowGameOver();
    }

    private static void HandleAddCard(byte[] pBuffer)
    {
        Debug.Log("HandleAddCard");
        Protocol.G2C_AddCard packet = G2C_AddCard.Parser.ParseFrom(pBuffer);

        HandManager.instance.AddCard(packet.CardData);
    }

    private static void HandleLeaveRoomNotificationPacket(byte[] pBuffer)
    {
        Debug.Log("HandleLeaveRoomNotificationPacket");
        Protocol.G2C_LeaveRoomNotification pkt = G2C_LeaveRoomNotification.Parser.ParseFrom(pBuffer);
        Debug.Log(pkt.UserId);
        LobbyManager.instance.uiRoom.RemoveUserFromSlot(pkt.UserId);
    }

    private static void HandleMonsterAttackBase(byte[] pBuffer)
    {
        G2C_MonsterAttackBaseNotification pkt = G2C_MonsterAttackBaseNotification.Parser.ParseFrom(pBuffer);
        MonsterManager.instance.HandleMonsterAttackTower(pkt.MonsterId);
        Base.instance.GetDamage(pkt.AttackDamage);
    }

    private static void HandleMonsterAttackTower(byte[] pBuffer)
    {
        G2C_MonsterAttackTowerNotification pkt = G2C_MonsterAttackTowerNotification.Parser.ParseFrom(pBuffer);
        Debug.Log(pkt.MonsterId);
        MonsterManager.instance.HandleMonsterAttackTower(pkt.MonsterId);

        Tower tower = TowerManager.instance.GetTowerByUuid(pkt.TargetId);

        if (tower != null)
        {
            tower.SetHp(pkt.Hp);
        }
        else
        {
            Debug.Log("타워가 널");
        }
    }

    /*---------------------------------------------
   [몬스터 이동 동기화]
---------------------------------------------*/
    static void HandleMonsterMove(byte[] pBuffer)
    {
        G2C_MonsterPositionUpdateNotification pkt = Protocol.G2C_MonsterPositionUpdateNotification.Parser.ParseFrom(pBuffer);

        MonsterManager.instance.HandleMonsterMove(pkt.PosInfo);
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
        Protocol.G2C_GetRoomListResponse pkt = Protocol.G2C_GetRoomListResponse.Parser.ParseFrom(pBuffer);

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
        Protocol.G2C_JoinRoomResponse pkt = Protocol.G2C_JoinRoomResponse.Parser.ParseFrom(pBuffer);
        Debug.Log(pkt.RoomInfo.Users.Count);

        Debug.Log(pkt.RoomInfo);
        LobbyManager.instance.OnEnteredRoom(pkt.RoomInfo);

    }
    static void HandleJoinRoomNotificationPacket(byte[] pBuffer)
    {
        Debug.Log("나 참여");
        //패킷 역직렬화
        Protocol.G2C_JoinRoomNotification pkt = Protocol.G2C_JoinRoomNotification.Parser.ParseFrom(pBuffer);

        LobbyManager.instance.OnJoinedRoomSomeone(pkt.JoinUser);
    }

    /*---------------------------------------------
    [방 생성]
---------------------------------------------*/
    static void HandleCreateRoomResponsePacket(byte[] pBuffer)
    {
        //패킷 역직렬화
        Protocol.G2C_CreateRoomResponse pkt = Protocol.G2C_CreateRoomResponse.Parser.ParseFrom(pBuffer);

        //방 입장 요청 보내기
        LobbyManager.instance.uiMain.OnClickJoinRoom(pkt.Room.Id);
    }

    /*---------------------------------------------
    [게임 시작 1/2]
    - 
---------------------------------------------*/
    static void HandleCreateGameRoomNotification(byte[] pBuffer)
    {
        Debug.Log("게시1");
        Protocol.G2C_CreateGameRoomNotification packet = G2C_CreateGameRoomNotification.Parser.ParseFrom(pBuffer);
        Protocol.C2G_JoinGameRoomRequest requestPacket = new C2G_JoinGameRoomRequest();
        requestPacket.ServerId = packet.ServerId;
        requestPacket.RoomId = PlayerInfoManager.instance.roomId;
        requestPacket.PlayerData = new GamePlayerData
        {
            Nickname = PlayerInfoManager.instance.nickname,
            PrefabId = PlayerInfoManager.instance.prefabId,
            Position = new PosInfo { Uuid = PlayerInfoManager.instance.userId }
        };

        byte[] sendBuffer = PacketUtils.SerializePacket(requestPacket, ePacketID.C2G_JoinGameRoomRequest, 0);
        NetworkManager.instance.SendPacket(sendBuffer);
    }

    /*---------------------------------------------
    [게임 시작 2/2]
    - 모든 플레이어가 접속하여 게임 시작
---------------------------------------------*/
    static void HandleBattleGameStart(byte[] pBuffer)
    {
        // 1. 패킷 파싱
        Protocol.G2C_GameStartNotification pkt = Protocol.G2C_GameStartNotification.Parser.ParseFrom(pBuffer);

        //temp
        PlayerInfoManager.instance.tmp_obstaclePosInfos = pkt.ObstaclePosInfos;
        // 2. PlayerManager에 데이터 저장
        PlayerManager.instance.ClearPlayers();
        foreach (var playerData in pkt.PlayerDatas)
        {
            Debug.Log(playerData);
            PlayerManager.instance.AddPlayer(playerData);
        }

        // 3. 게임 씬으로 전환
        SceneChanger.ChangeScene(SceneChanger.SceneType.TestGame);

        // 4. 씬 전환 후 캐릭터, 장애물 초기화 (중복 등록 방지)
        SceneChanger.OnSceneLoaded -= InitializeCharacters; // 기존 이벤트 제거
        SceneChanger.OnSceneLoaded += InitializeCharacters; // 새로운 이벤트 등록
    }

    private static void HandleIncreaseWaveNotification(byte[] pBuffer)
    {
        G2C_TowerDestroyNotification packet = Protocol.G2C_TowerDestroyNotification.Parser.ParseFrom(pBuffer);
        if (packet.IsSuccess)
        {
            Debug.Log("다음 웨이브");
            ScoreManager.instance.AddWave();
        }
    }

    // 캐릭터 초기화 메서드 (독립적인 메서드로 분리)
    private static async void InitializeCharacters()
    {
        Debug.Log("게임 씬 로드 완료. 캐릭터 초기화 시작");
        await CharacterManager.instance.InitializeCharacters();

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
            Protocol.G2C_PlayerPositionUpdateNotification response = Protocol.G2C_PlayerPositionUpdateNotification.Parser.ParseFrom(pBuffer);

            // 2. 단일 위치 정보 처리
            var posInfo = response.PosInfo;
            //Debug.Log("HandleMove" + posInfo.X + ", " + posInfo.Y);
            // 3. 캐릭터 검색
            Character character = CharacterManager.instance.GetCharacter(posInfo.Uuid);

            if (character != null)
            {
                // 로컬 플레이어가 아닌 경우에만 업데이트
                if (!character.isLocalPlayer)
                {
                    character.UpdatePositionFromServer(posInfo.X, posInfo.Y, response.Parameter, response.State);
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

    // 캐릭터 애니메이션 동기화
    // static void HandleCharacterAnimation(byte[] pBuffer)
    // {
    //     Protocol.B2C_PlayerAnimationUpdateNotification packet = Protocol.B2C_PlayerAnimationUpdateNotification.Parser.ParseFrom(pBuffer);
    //     Debug.Log("HandleCharacterAnimation Called: packet: " + packet);

    //     Character character = CharacterManager.instance.GetCharacter(packet.CharacterId);

    //     if (character != null)
    //     {
    //         character.UpdateAnimationFromServer(packet.Parameter, packet.State);
    //     }
    // }

    static void HandleSpawnMonster(byte[] pBuffer)
    {
        Debug.Log("HandleSpawnMonster Called");

        G2C_SpawnMonsterNotification packet = Protocol.G2C_SpawnMonsterNotification.Parser.ParseFrom(pBuffer);

        MonsterManager.instance.SpawnMonster(packet.PrefabId, packet.PosInfo, packet.MaxHp);
    }

    static void HandleMonsterHealthUpdateNotification(byte[] pBuffer)
    {
        Debug.Log("HandleMonsterHealthUpdateNotification Called");

        G2C_MonsterHealthUpdateNotification packet = Protocol.G2C_MonsterHealthUpdateNotification.Parser.ParseFrom(pBuffer);

        Monster monster = MonsterManager.instance.GetMonsterByUuid(packet.MonsterId);
        monster.SetHp(packet.Hp);
    }

    static void HandleMonsterDeath(byte[] pBuffer)
    {
        Debug.Log("HandleMonsterDeath Called");

        G2C_MonsterDeathNotification packet = Protocol.G2C_MonsterDeathNotification.Parser.ParseFrom(pBuffer);

        Debug.Log("Monster Death: MonsterId: " + packet.MonsterId);
        Monster monster = MonsterManager.instance.GetMonsterByUuid(packet.MonsterId);
        monster.Die();
        MonsterManager.instance.RemoveMonster(packet.MonsterId);
        ScoreManager.instance.AddScore(packet.Score);

    }

    static void HandleBuildTowerNotification(byte[] pBuffer)
    {
        Debug.Log("HandleBuildTowerNotification Called");

        G2C_TowerBuildNotification packet = Protocol.G2C_TowerBuildNotification.Parser.ParseFrom(pBuffer);

        TowerPlacementManager.instance.BuildTower(packet.OwnerId, packet.Tower, packet.MaxHp);
    }

    static void HandleTowerAttackMonsterNotification(byte[] pBuffer)
    {
        Debug.Log("HandleTowerAttackMonsterNotification");

        G2C_TowerAttackMonsterNotification packet = Protocol.G2C_TowerAttackMonsterNotification.Parser.ParseFrom(pBuffer);

        Tower tower = TowerManager.instance.GetTowerByUuid(packet.TowerId);
        if (tower != null)
        {
            tower.AttackTarget(packet.MonsterPos, packet.TravelTime);
        }
        else
        {
            Debug.Log(packet.MonsterPos);
        }
    }

    static void HandleTowerHealthUpdateNotification(byte[] pBuffer)
    {
        Debug.Log("HandleTowerHealthUpdateNotification Called");

        G2C_TowerHealthUpdateNotification packet = Protocol.G2C_TowerHealthUpdateNotification.Parser.ParseFrom(pBuffer);

        Tower tower = TowerManager.instance.GetTowerByUuid(packet.TowerId);
        if (tower != null)
        {
            tower.SetHp(packet.Hp);
        }
    }

    static void HandleTowerDestroyNotification(byte[] pBuffer)
    {
        Debug.Log("HandleTowerDestroyNotification Called");

        G2C_TowerDestroyNotification packet = Protocol.G2C_TowerDestroyNotification.Parser.ParseFrom(pBuffer);

        Tower tower = TowerManager.instance.GetTowerByUuid(packet.TowerId);
        if (tower != null)
        {
            tower.Destroy();
        }
        else
        {
            Debug.Log("타워가 존재하지 않습니다.");
            return;
        }

        TowerManager.instance.RemoveTower(packet.TowerId);
    }

    static void HandleUseSkillNotification(byte[] pBuffer)
    {
        Debug.Log("HandleUseSkillNotification Called");

        G2C_UseSkillNotification packet = Protocol.G2C_UseSkillNotification.Parser.ParseFrom(pBuffer);

        SkillManager.instance.UseSkill(packet.OwnerId, packet.Skill);
    }

    static void HandleInitCardData(byte[] pBuffer)
    {
        //Debug.Log("HandleInitCardData Called");

        G2C_InitCardData packet = Protocol.G2C_InitCardData.Parser.ParseFrom(pBuffer);

        HandManager.instance.AddInitCard(packet.CardData);
    }

    static void HandleSkillResponse(byte[] pBuffer)
    {

    }

    static void HandleTowerBuffNotification(byte[] pBuffer)
    {
        Debug.Log("HandleTowerBuffNotification Called");


        G2C_TowerBuffNotification packet = Protocol.G2C_TowerBuffNotification.Parser.ParseFrom(pBuffer);

        foreach (string towerId in packet.TowerId)
        {
            TowerManager.instance.GetTowerByUuid(towerId).SetBuffEffect(packet.BuffType, packet.IsBuffed);
        }
    }

    static void HandlePlayerUseAbilityNotification(byte[] pBuffer)
    {
        //Debug.Log("HandlePlayerUseAbilityNotification Called");

        G2C_PlayerUseAbilityNotification packet = Protocol.G2C_PlayerUseAbilityNotification.Parser.ParseFrom(pBuffer);

        AbilityManager.instance.HandleAbility(packet.PrefabId, packet.Position);
    }

    // 채팅
    static void HandleChatMessageNotification(byte[] pBuffer)
    {
        G2C_ChatMessageNotification packet = Protocol.G2C_ChatMessageNotification.Parser.ParseFrom(pBuffer);

        Character character = CharacterManager.instance.GetCharacter(packet.UserId);

        ChatManager.instance.AddMessageOnDisPlay(character.nickname, packet.Message);
    }
}

