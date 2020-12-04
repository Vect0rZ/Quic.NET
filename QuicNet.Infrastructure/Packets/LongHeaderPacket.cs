using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class LongHeaderPacket : Packet
    {
        public override byte Type => 0b1100_0000; // 1100 0000

        public byte DestinationConnectionIdLength { get; set; }
        public GranularInteger DestinationConnectionId { get; set; }
        public byte SourceConnectionIdLength { get; set; }
        public GranularInteger SourceConnectionId { get; set; }

        public PacketType PacketType { get; set; }

        public LongHeaderPacket()
        {

        }

        public LongHeaderPacket(PacketType packetType, GranularInteger destinationConnectionId, GranularInteger sourceConnectionId)
        {
            PacketType = packetType;
            DestinationConnectionIdLength = destinationConnectionId.Size;
            DestinationConnectionId = destinationConnectionId;

            SourceConnectionIdLength = sourceConnectionId.Size;
            SourceConnectionId = sourceConnectionId;
        }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);

            byte type = array.ReadByte();
            PacketType = DecodeTypeFiled(type);

            Version = array.ReadUInt32();

            DestinationConnectionIdLength = array.ReadByte();
            if (DestinationConnectionIdLength > 0)
                DestinationConnectionId = array.ReadGranularInteger(DestinationConnectionIdLength);

            SourceConnectionIdLength = array.ReadByte();
            if (SourceConnectionIdLength > 0)
                SourceConnectionId = array.ReadGranularInteger(SourceConnectionIdLength);

            this.DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();

            result.Add(EncodeTypeField());
            result.AddRange(ByteUtilities.GetBytes(Version));

            result.Add(DestinationConnectionId.Size);
            if (DestinationConnectionId.Size > 0)
                result.AddRange(DestinationConnectionId.ToByteArray());

            result.Add(SourceConnectionId.Size);
            if (SourceConnectionId.Size > 0)
                result.AddRange(SourceConnectionId.ToByteArray());

            result.AddRange(frames);

            return result.ToArray();
        }

        private byte EncodeTypeField()
        {
            byte type = (byte)(Type | ((byte)PacketType << 4) & 0b0011_0000);

            return type;
        }

        private PacketType DecodeTypeFiled(byte type)
        {
            PacketType result = (PacketType)((type & 0b0011_0000) >> 4);

            return result;
        }
    }
}
