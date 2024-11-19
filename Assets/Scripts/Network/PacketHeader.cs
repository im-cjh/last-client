using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ePacketID : UInt16
{
    S2C_Error = 0,
    C2L_Init = 1,
    C2L_EnterRoom = 2,
    C2L_LeaveRoom = 3,
    C2L_GetRooms = 4,
    C2L_GameStart = 5,
    C2L_CreateRoom = 6,
    C2B_Init = 21,
    C2B_Move = 22,
    L2C_Init = 51,
    L2C_JoinRoomResponse = 52,
    L2C_JoinRoomNotification = 53,
    L2C_LeaveRoomMe = 54,
    L2C_LeaveRoomOther = 55,
    L2C_GetRoom = 56,
    L2C_GameStart = 57,
    L2C_CreateRoomResponse = 58,
    L2B_Init = 61,
    L2B_CreateRoom = 62,
    B2C_Enter = 101,
    B2C_GameStart = 102,
    B2C_Move = 103,
    B2L_Init = 121,
    B2L_CreateRoom = 122,
}

[Serializable]
public struct PacketHeader
{
    public Int16 size;
    public ePacketID id; // 프로토콜ID 
    public Int32 sequence;
}