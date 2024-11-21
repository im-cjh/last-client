using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    private TcpClient mLobbyTcpClient;
    private NetworkStream mLobbyStream;

    private TcpClient mBattleTcpClient;
    private NetworkStream mBattleStream;

    WaitForSecondsRealtime wait;

    private byte[] mRecvBuffer = new byte[4096];
    private List<byte> incompleteData = new List<byte>();

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        wait = new WaitForSecondsRealtime(5);
    }

    private void Start()
    {
        string ip = "127.0.0.1";
        int port = 3000;

        if (ConnectToLobbyServer(ip, port))
        {
            StartGame();
        }
    }

    bool ConnectToLobbyServer(string ip, int port)
    {
        try
        {
            mLobbyTcpClient = new TcpClient(ip, port);
            mLobbyStream = mLobbyTcpClient.GetStream();
            Debug.Log($"Connected to {ip}:{port}");

            return true;
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
            return false;
        }
    }
    public bool ConnectToBattleServer(string ip, int port, int pRoomId)
    {
        Debug.Log("ConnectToBattleServer");
        try
        {
            mBattleTcpClient = new TcpClient(ip, port);
            mBattleStream = mBattleTcpClient.GetStream();
            Debug.Log($"Connected to {ip}:{port}");

            StartBattleReceiving();

            Protocol.C2B_Init pkt = new Protocol.C2B_Init();
            
            pkt.RoomId = pRoomId;
            pkt.PlayerData = new Protocol.GamePlayerData
            {
                Position = new Protocol.PosInfo
                {
                    Uuid = PlayerInfoManager.instance.userId,
                    X = 0,
                    Y = 0,
                },
                Nickname= PlayerInfoManager.instance.nickname,
                CharacterType = PlayerInfoManager.instance.characterData.CharacterType,
            };

            Debug.Log(pkt);
            byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2B_Init, 0);
            SendBattlePacket(sendBuffer);
            return true;
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
            return false;
        }
    }
    void StartGame()
    {
        // 게임 시작 코드 작성
        StartLobbyReceiving(); // Start receiving data
        SendInitialPacket();
    }
    public async void SendLobbyPacket(byte[] sendBuffer)
    {

        await Task.Delay(PlayerInfoManager.instance.latency);

        // 패킷 전송
        mLobbyStream.Write(sendBuffer, 0, sendBuffer.Length);
    }
    public async void SendBattlePacket(byte[] sendBuffer)
    {
        await Task.Delay(PlayerInfoManager.instance.latency);

        // 패킷 전송
        mBattleStream.Write(sendBuffer, 0, sendBuffer.Length);
    }

    void SendInitialPacket()
    {
        Protocol.C2L_Init pkt = new Protocol.C2L_Init();
        pkt.UserId = PlayerInfoManager.instance.userId;
        pkt.Nickname = PlayerInfoManager.instance.nickname;

        byte[] sendBuffer = PacketUtils.SerializePacket(pkt, ePacketID.C2L_Init, PlayerInfoManager.instance.GetNextSequence());
        
        SendLobbyPacket(sendBuffer);
    }
    void StartLobbyReceiving()
    {
        _ = RecvLobbyPacketsAsync();
    }

    void StartBattleReceiving()
    {
        _ = RecvBattlePacketsAsync();
    }

    /*---------------------------------------------
[RegisterRecv]
-로비서버
---------------------------------------------*/
    async System.Threading.Tasks.Task RecvLobbyPacketsAsync()
    {
        while (mLobbyTcpClient.Connected)
        {
            try
            {
                int bytesRead = await mLobbyStream.ReadAsync(mRecvBuffer, 0, mRecvBuffer.Length);
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
[RegisterRecv]
    -배틀서버
---------------------------------------------*/
    async System.Threading.Tasks.Task RecvBattlePacketsAsync()
    {
        while (mBattleTcpClient.Connected)
        {
            try
            {
                int bytesRead = await mBattleStream.ReadAsync(mRecvBuffer, 0, mRecvBuffer.Length);
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
            HandlePacket(packetData, header.id);
        }
    }

    /*---------------------------------------------
[handlePacket]
- 목적: 수신한 패킷의 Id에 맞는 함수 호출

1. 패킷 ID에 해당하는 핸들러 확인
1-1. 핸들러가 존재하지 않을 경우 오류 출력
2. 핸들러 호출
---------------------------------------------*/
    private void HandlePacket(byte[] pBuffer, ePacketID pId)
    {
        //핸들러가 존재하지 않을 경우 오류 출력
        Action<byte[]> handler;
        try
        {
            handler = PacketHandler.handlerMapping[pId];
        }
        catch (Exception e)
        {
            Debug.Log("패킷id가 잘못되었습니다: " + pId);
            return; //throw e;
        }
        //핸들러 호출
        try
        {
            handler(pBuffer);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return; //throw e;
        }
    }
}
