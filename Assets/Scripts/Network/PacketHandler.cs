using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //handlerMapping[ePacketID.L2C_GetRoom] = HandleRoomsPacket;
        //handlerMapping[ePacketID.L2C_EnterRoomMe] = HandleEnterRoomMe;
        //handlerMapping[ePacketID.L2C_EnterRoomOther] = HandleEnterRoomOther;
        //handlerMapping[ePacketID.L2C_GameStart] = HandleLobbyGameStart;
        //handlerMapping[ePacketID.B2C_GameStart] = HandleBattleGameStart;
        //handlerMapping[ePacketID.B2C_Enter] = HandleEnterGame;
        //handlerMapping[ePacketID.B2C_Move] = HandleMove;
    }

    static void HandleInitPacket(byte[] pBuffer)
    {
        //패킷 역직렬화
        Protocol.L2C_Init pkt = Protocol.L2C_Init.Parser.ParseFrom(pBuffer);
        PlayerInfoManager.instance.playerId = pkt.UserId;
        //임시
        SceneChanger.ChangeLobbyScene();
        //GameManager.instance.GameStart();
    }
}
