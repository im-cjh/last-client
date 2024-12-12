using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ePacketID : UInt16
{
    S2C_Error = 0,
    C2L_Init = 1,
    L2C_Init = 2,
    B2L_Init = 3,
    C2B_Init = 4,
    C2L_CreateRoomRequest = 101,
    L2C_CreateRoomResponse = 102,
    L2B_CreateGameRoomRequest = 103,
    B2L_CreateGameRoomRespone = 104,
    C2L_GameStart = 105,
    L2C_GameStart = 106,
    L2B_GameStartRequest = 107,
    B2C_GameStartNotification = 108,
    C2L_GetRoomListRequest = 109,
    L2C_GetRoomListResponse = 110,
    C2L_JoinRoomRequest = 111,
    L2C_JoinRoomResponse = 112,
    L2C_JoinRoomNotification = 113,
    C2L_LeaveRoomRequest = 114,
    L2C_LeaveRoomResponse = 115,
    L2C_LeaveRoomNotification = 116,
    B2C_JoinRoomResponse = 117,
    B2C_increaseWaveNotification = 118,
    B2C_SpawnMonsterNotification = 201,
    B2C_MonsterPositionUpdateNotification = 202,
    B2C_MonsterAttackTowerNotification = 203,
    B2C_MonsterAttackBaseNotification = 204,
    B2C_MonsterDeathNotification = 205,
    C2B_TowerBuildRequest = 301,
    B2C_TowerBuildResponse = 302,
    B2C_TowerBuildNotification = 303,
    B2C_TowerAttackMonsterNotification = 304,
    B2C_TowerDestroyNotification = 305,
    B2C_BaseDestroyNotification = 306,
    B2C_ObstacleSpawnNotification = 307,
    B2C_TowerHealthUpdateNotification = 308,
    C2B_SkillRequest = 401,
    B2C_SkillResponse = 402,
    B2C_SkillNotify = 403,
    C2B_PositionUpdateRequest = 501,
    B2C_PlayerPositionUpdateNotification = 502,
    C2B_UseCardRequest = 503,
    B2C_UseSkillNotification = 504,
    B2C_MonsterHealthUpdateNotification = 505,
    B2C_InitCardData = 506,
    B2C_AddCard = 507,
    B2L_SocketDisconnectedNotification = 508,
    B2C_GameEndNotification = 509,
    B2C_MonsterBuffNotification = 601,
}

[Serializable]
public struct PacketHeader
{
    public Int16 size;
    public ePacketID id; // 프로토콜ID 
    public Int32 sequence;
}