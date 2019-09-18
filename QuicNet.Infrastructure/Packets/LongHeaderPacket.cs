using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class LongHeaderPacket : Packet
    {
        public override byte Type => 0xC0; // 1100 0000

        public byte DCID { get; set; }
        public byte DestinationConnectionId { get; set; }
        public byte SCID { get; set; }
        public byte SourceConnectionId { get; set; }

        public PacketType PacketType { get; set; }
        public LongHeaderPacket(PacketType packetType)
        {
            PacketType = packetType;
        }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            PacketType = (PacketType)(type & 0x30);

            Version = array.ReadUInt32();

            this.DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();

            result.Add(EncodeTypeField());
            result.AddRange(ByteUtilities.GetBytes(Version));

            if (DCID > 0)
                result.Add(DestinationConnectionId);
            if (SCID > 0)
                result.Add(SourceConnectionId);

            result.AddRange(frames);

            return result.ToArray();
        }

        private byte EncodeTypeField()
        {
            byte type = (byte)(Type | (byte)PacketType | 0x03);

            return type;
        }
    }
}
