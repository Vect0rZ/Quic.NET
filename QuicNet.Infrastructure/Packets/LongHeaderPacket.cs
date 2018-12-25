using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class LongHeaderPacket : Packet
    {
        public override byte Type => 0xC0;

        public byte DCIL_SCIL { get; set; }
        public byte DestinationConnectionId { get; set; }
        public byte SourceConnectionId { get; set; }
        public VariableInteger TokenLength { get; set; }
        public byte[] Token { get; set; }
        public VariableInteger Length { get; set; }
        public UInt32 PacketNumber { get; set; }

        public PacketType PacketType { get; set; }
        public LongHeaderPacket(PacketType packetType)
        {
            DCIL_SCIL = 0;
            PacketType = packetType;
        }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            PacketType = (PacketType)(type & 0x30);

            Version = array.ReadUInt32();
            DCIL_SCIL = array.ReadByte();

            if ((DCIL_SCIL & 0xF0) != 0)
                DestinationConnectionId = array.ReadByte();
            if ((DCIL_SCIL & 0x0F) != 0)
                SourceConnectionId = array.ReadByte();

            TokenLength = array.ReadVariableInteger();
            if (TokenLength != 0)
                Token = array.ReadBytes((int)TokenLength.Value);

            Length = array.ReadVariableInteger();
            PacketNumber = array.ReadUInt32();

            Length = Length - 4;

            this.DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();
            result.Add(EncodeTypeField());
            result.AddRange(ByteUtilities.GetBytes(Version));

            if (DestinationConnectionId > 0)
                DCIL_SCIL = (byte)(DCIL_SCIL | 0x50);
            if (SourceConnectionId > 0)
                DCIL_SCIL = (byte)(DCIL_SCIL | 0x05);

            result.Add(DCIL_SCIL);

            if (DestinationConnectionId > 0)
                result.Add(DestinationConnectionId);
            if (SourceConnectionId > 0)
                result.Add(SourceConnectionId);

            byte[] tokenLength = new VariableInteger(0);
            byte[] length = new VariableInteger(4 + (UInt64)frames.Length);

            result.AddRange(tokenLength);
            result.AddRange(length);
            result.AddRange(ByteUtilities.GetBytes(PacketNumber));
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
