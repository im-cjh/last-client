using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PacketUtils
{
    public static byte[] SerializePacket<T>(T pPkt, ePacketID pMessageID, int pSequence) where T : IMessage
    {
        int size = pPkt.CalculateSize() + Marshal.SizeOf(typeof(PacketHeader));
        PacketHeader header = new PacketHeader();
        header.size = (ushort)size;
        header.id = pMessageID;
        header.sequence = pSequence;

        //Debug.Log("크기: "+ pPkt.CalculateSize()+" : "+ Marshal.SizeOf(typeof(PacketHeader)));

        using (MemoryStream ret = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ret, Encoding.Default, true))
            {
                //패킷 헤더 직렬화
                writer.Write(header.size);
                writer.Write((UInt16)header.id);
                writer.Write(header.sequence);
            }
            pPkt.WriteTo(ret);

            return ret.ToArray();
        }

    }
}

