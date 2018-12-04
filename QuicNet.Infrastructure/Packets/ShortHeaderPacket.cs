using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class ShortHeaderPacket : Packet
    {
        public override byte Type => 0x32; // 00110010;
        public byte DestinationConnectionId { get; set; }
        public UInt32 PacketNumber { get; set; }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            PacketNumber = array.ReadUInt32();

            DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();
            result.Add(Type);
            result.AddRange(ByteUtilities.GetBytes(PacketNumber));
            result.AddRange(frames);

            return result.ToArray();
        }
    }
}
