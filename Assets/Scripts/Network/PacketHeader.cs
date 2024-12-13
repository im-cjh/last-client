using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ePacketID : UInt16
{
    S2C_Error = 0,
    C2G_Init = 1,
    G2L_Init = 2,
    G2B_Init = 3,
    C2G_CreateRoomRequest = 101,
    G2L_CreateRoomRequest = 102,
    L2G_CreateRoomResponse = 103,
    G2C_CreateRoomResponse = 104,
    G2B_CreateGameRoomRequest = 105,
    B2G_CreateGameRoomResponse = 106,
    G2C_CreateGameRoomNotification = 107,
    C2G_GameStartRequest = 108,
    G2L_GameStartRequest = 109,
    B2G_GameStartNotification = 110,
    G2C_GameStartNotification = 111,

    C2G_GetRoomListRequest = 112,
    G2L_GetRoomListRequest = 113,
    L2G_GetRoomListResponse = 114,
    G2C_GetRoomListResponse = 115,
    C2G_JoinRoomRequest = 116,
    G2L_JoinRoomRequest = 117,
    L2G_JoinRoomResponse = 118,
    G2C_JoinRoomResponse = 119,
    L2G_JoinRoomNotification = 120,
    G2C_JoinRoomNotification = 121,
    //구현X - 시작
    C2G_LeaveRoomRequest = 122,
    G2L_LeaveRoomRequest = 123,
    L2G_LeaveRoomResponse = 124,
    G2C_LeaveRoomResponse = 125,
    L2G_LeaveRoomNotification = 126,
    G2C_LeaveRoomNotification = 127,
    //구현X - 끝
    C2G_JoinGameRoomRequest = 128,
    G2B_JoinGameRoomRequest = 129,
    B2G_JoinGameRoomResponse = 130,
    G2C_JoinGameRoomResponse = 131,
    //
    // B2C_increaseWaveNotification = 118,
    B2G_SpawnMonsterNotification = 201,
    G2C_SpawnMonsterNotification = 202,
    B2G_MonsterPositionUpdateNotification = 203,
    G2C_MonsterPositionUpdateNotification = 204,
    B2G_MonsterAttackTowerNotification = 205,
    G2C_MonsterAttackTowerNotification = 206,
    B2G_MonsterAttackBaseNotification = 207,
    G2C_MonsterAttackBaseNotification = 208,
    B2G_MonsterDeathNotification = 209,
    G2C_MonsterDeathNotification = 210,
    C2G_TowerBuildRequest = 301,
    G2B_TowerBuildRequest = 302,
    B2G_TowerBuildNotification = 303,
    G2C_TowerBuildNotification = 304,
    B2G_TowerAttackMonsterNotification = 305,
    G2C_TowerAttackMonsterNotification = 306,
    B2G_TowerDestroyNotification = 305,
    G2C_TowerDestroyNotification = 305,
    B2G_BaseDestroyNotification = 306,
    G2C_BaseDestroyNotification = 307,
    B2G_TowerHealthUpdateNotification = 308,
    G2C_TowerHealthUpdateNotification = 309,
    // C2G_UseSkillRequest = 401,
    // G2B_UseSkillRequest = 402,
    // B2G_UseSkillResponse = 403,
    // B2G_UseSkillNotification = 404,
    C2G_PlayerPositionUpdateRequest = 501,
    G2B_PlayerPositionUpdateRequest = 502,
    B2G_PlayerPositionUpdateNotification = 503,
    G2C_PlayerPositionUpdateNotification = 504,
    B2G_MonsterHealthUpdateNotification = 505,
    G2C_MonsterHealthUpdateNotification = 506,
    B2G_InitCardData = 507,
    G2C_InitCardData = 508
    //B2G_AddCard = 507,
    //B2L_SocketDisconnectedNotification = 508,
    //B2C_GameEndNotification = 509,
}

[Serializable]
public struct PacketHeader
{
    public Int16 size;
    public ePacketID id; // 프로토콜ID 
    public Int32 sequence;
}