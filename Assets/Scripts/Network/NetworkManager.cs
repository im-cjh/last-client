using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    private TcpClient mTcpClient;
    private NetworkStream mGatewayStream;


    private byte[] mRecvBuffer = new byte[4096];
    private List<byte> incompleteData = new List<byte>();

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return; // 추가 실행 방지
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    public void ConnectToGatewayServer(string ip = "127.0.0.1", int port = 9000)
    //public void ConnectToGatewayServer(string ip = "ec2-52-79-226-206.ap-northeast-2.compute.amazonaws.com", int port = 9000)
    {
        try
        {
            mTcpClient = new TcpClient(ip, port);
            mGatewayStream = mTcpClient.GetStream();
            UnityEngine.Debug.Log("게이트웨이 서버 연결");

            StartGame();
        }
        catch (SocketException e)
        {
            UnityEngine.Debug.LogError($"SocketException: {e}");
        }
    }

    void StartGame()
    {
        // 게임 시작 코드 작성
        StartLobbyReceiving(); // Start receiving data
        SendInitialPacket();
    }
    public async void SendPacket(byte[] sendBuffer)
    {
        await Task.Delay(PlayerInfoManager.instance.latency);
        // 패킷 전송
        mGatewayStream.Write(sendBuffer, 0, sendBuffer.Length);
    }

    void SendInitialPacket()
    {
        try
        {
            Protocol.C2G_Init pkt = new Protocol.C2G_Init();
            pkt.Token = PlayerInfoManager.instance.token;

            byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2G_Init, PlayerInfoManager.instance.GetNextSequence());

            SendPacket(sendBuffer);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    void StartLobbyReceiving()
    {
        _ = RecvLobbyPacketsAsync();
    }

    public void Logout()
    {
        mGatewayStream.Close();
        mGatewayStream = null;

        mTcpClient.Close();
        mTcpClient = null;

        mRecvBuffer = new byte[4096];
        incompleteData.Clear(); // 패킷 조각 리스트 초기화
    }
    /*---------------------------------------------
[RegisterRecv]
-로비서버
---------------------------------------------*/
    async System.Threading.Tasks.Task RecvLobbyPacketsAsync()
    {
        while (mTcpClient.Connected)
        {
            try
            {
                int bytesRead = await mGatewayStream.ReadAsync(mRecvBuffer, 0, mRecvBuffer.Length);
                if (bytesRead > 0)
                {
                    OnData(mRecvBuffer, bytesRead);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Receive error: {e.Message}");
                break;
            }
        }
    }

    /*---------------------------------------------
[OnData] 
1. 온전한 패킷이 왔는지 확인
2. 패킷 추출(byte[])
3. PacketHeader를 읽어서 ePacket에 대응되는 핸들러 함수 호출
---------------------------------------------*/
    void OnData(byte[] data, int length)
    {
        incompleteData.AddRange(data.AsSpan(0, length).ToArray());

        //Debug.Log("ProcessReceivedData" + incompleteData.Count);
        //헤더는 읽을 수 있음
        while (incompleteData.Count >= Marshal.SizeOf(typeof(PacketHeader)))
        {
            // 패킷 길이와 타입 읽기
            //서버에서 subaray한거랑 비슷한듯 ㅎㅎ
            byte[] lengthBytes = incompleteData.GetRange(0, Marshal.SizeOf(typeof(PacketHeader))).ToArray();
            PacketHeader header = MemoryMarshal.Read<PacketHeader>(lengthBytes);

            // 헤더에 기록된 패킷 크기를 파싱할 수 있어야 한다
            if (incompleteData.Count < header.size)
            {
                //Debug.Log("데이터가 충분하지 않으면 반환" + incompleteData.Count + " : " + header.size);
                return;
            }

            // 패킷 데이터 추출
            byte[] packetData = incompleteData.GetRange(Marshal.SizeOf(typeof(PacketHeader)), header.size - Marshal.SizeOf(typeof(PacketHeader))).ToArray();
            incompleteData.RemoveRange(0, header.size);

            // Debug.Log($"Received packet: Length = {packetLength}, Type = {packetType}");
            _ = HandlePacket(packetData, header.id);
        }
    }

    /*---------------------------------------------
[handlePacket]
- 목적: 수신한 패킷의 Id에 맞는 함수 호출

1. 패킷 ID에 해당하는 핸들러 확인
1-1. 핸들러가 존재하지 않을 경우 오류 출력
2. 핸들러 호출
---------------------------------------------*/
    private async Task HandlePacket(byte[] pBuffer, ePacketID pId)
    {
        Debug.Log(pId);
        try
        {
            // 동기 핸들러 검색
            if (PacketHandler.handlerMapping.TryGetValue(pId, out var syncHandler))
            {
                Debug.Log("찾음");
                try
                {
                    syncHandler(pBuffer);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError($"동기 핸들러 처리 중 오류 발생. ID: {pId}, 오류: {e}");
                    return;
                }
            }
            else
            {
                Debug.Log("못찾음");
            }
        }
        catch(Exception e)
        {
            Debug.LogError (e);
        }

        // 비동기 핸들러 검색
        if (PacketHandler.asyncHandlerMapping.TryGetValue(pId, out var asyncHandler))
        {
            try
            {
                await asyncHandler(pBuffer);
                return;
            }
            catch (Exception e)
            {
                Debug.LogError($"비동기 핸들러 처리 중 오류 발생. ID: {pId}, 오류: {e}");
                return;
            }
        }

        // 핸들러가 없는 경우
        Debug.LogWarning($"핸들러를 찾을 수 없습니다. 잘못된 패킷 ID: {pId}");
    }

}
