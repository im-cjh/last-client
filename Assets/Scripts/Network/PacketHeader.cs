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
    C2B_SpawnMonsterRequest = 201,
    B2C_SpawnMonsterResponse = 202,
    S2B_SpawnMonsterNotification = 203,
    B2S_MonsterAttackTowerRequest = 204,
    S2B_UpdateTowerHPNotification = 205,
    B2S_MonsterAttackBaseRequest = 206,
    C2B_MonsterDeathRequest = 207,
    B2C_MonsterDeathNotification = 208,
    C2B_TowerBuildRequest = 301,
    B2C_TowerBuildResponse = 302,
    B2C_AddTowerNotification = 303,
    B2C_TowerAttackRequest = 304,
    B2C_TowerAttackNotification = 305,
    C2B_TowerDestroyRequest = 306,
    C2B_TowerDestroyResponse = 307,
    C2B_TowerDestroyNotification = 308,
    C2B_SkillRequest = 401,
    C2B_SkillResponse = 402,
    C2B_SkillNotify = 403,
    C2B_PositionUpdateRequest = 501,
    B2C_PositionUpdateNotification = 502,
    C2B_UseCardRequest = 503,
    B2C_UseCardNotification = 504
}

[Serializable]
public struct PacketHeader
{
    public Int16 size;
    public ePacketID id; // 프로토콜ID 
    public Int32 sequence;
}