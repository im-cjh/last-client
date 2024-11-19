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
        Int16 size = (Int16)(pPkt.CalculateSize() + Marshal.SizeOf(typeof(PacketHeader)));
        PacketHeader header = new PacketHeader();
        header.size = size;
        header.id = pMessageID;
        header.sequence = pSequence;


        using (MemoryStream ret = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ret, Encoding.Default, true))
            {
                //패킷 헤더 직렬화
                writer.Write((UInt16)header.size);
                writer.Write((UInt16)header.id);
                writer.Write(header.sequence);
            }
            pPkt.WriteTo(ret);

            return ret.ToArray();
        }

    }
}

