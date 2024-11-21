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
        // ���� ���� �ڵ� �ۼ�
        StartLobbyReceiving(); // Start receiving data
        SendInitialPacket();
    }
    public async void SendLobbyPacket(byte[] sendBuffer)
    {

        await Task.Delay(PlayerInfoManager.instance.latency);

        // ��Ŷ ����
        mLobbyStream.Write(sendBuffer, 0, sendBuffer.Length);
    }
    public async void SendBattlePacket(byte[] sendBuffer)
    {
        await Task.Delay(PlayerInfoManager.instance.latency);

        // ��Ŷ ����
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
-�κ񼭹�
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
    -��Ʋ����
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
1. ������ ��Ŷ�� �Դ��� Ȯ��
2. ��Ŷ ����(byte[])
3. PacketHeader�� �о ePacket�� �����Ǵ� �ڵ鷯 �Լ� ȣ��
---------------------------------------------*/
    void OnData(byte[] data, int length)
    {
        incompleteData.AddRange(data.AsSpan(0, length).ToArray());

        //Debug.Log("ProcessReceivedData" + incompleteData.Count);
        //����� ���� �� ����
        while (incompleteData.Count >= Marshal.SizeOf(typeof(PacketHeader)))
        {
            // ��Ŷ ���̿� Ÿ�� �б�
            //�������� subaray�ѰŶ� ����ѵ� ����
            byte[] lengthBytes = incompleteData.GetRange(0, Marshal.SizeOf(typeof(PacketHeader))).ToArray();
            PacketHeader header = MemoryMarshal.Read<PacketHeader>(lengthBytes);

            // ����� ��ϵ� ��Ŷ ũ�⸦ �Ľ��� �� �־�� �Ѵ�
            if (incompleteData.Count < header.size)
            {
                //Debug.Log("�����Ͱ� ������� ������ ��ȯ" + incompleteData.Count + " : " + header.size);
                return;
            }

            // ��Ŷ ������ ����
            byte[] packetData = incompleteData.GetRange(Marshal.SizeOf(typeof(PacketHeader)), header.size - Marshal.SizeOf(typeof(PacketHeader))).ToArray();
            incompleteData.RemoveRange(0, header.size);

            // Debug.Log($"Received packet: Length = {packetLength}, Type = {packetType}");
            HandlePacket(packetData, header.id);
        }
    }

    /*---------------------------------------------
[handlePacket]
- ����: ������ ��Ŷ�� Id�� �´� �Լ� ȣ��

1. ��Ŷ ID�� �ش��ϴ� �ڵ鷯 Ȯ��
1-1. �ڵ鷯�� �������� ���� ��� ���� ���
2. �ڵ鷯 ȣ��
---------------------------------------------*/
    private void HandlePacket(byte[] pBuffer, ePacketID pId)
    {
        //�ڵ鷯�� �������� ���� ��� ���� ���
        Action<byte[]> handler;
        try
        {
            handler = PacketHandler.handlerMapping[pId];
        }
        catch (Exception e)
        {
            Debug.Log("��Ŷid�� �߸��Ǿ����ϴ�: " + pId);
            return; //throw e;
        }
        //�ڵ鷯 ȣ��
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
