using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class InitialPacket : Packet
    {
        public override byte Type => 0xDC; // 1101 1100
        
        public byte DCID { get; set; }
        public byte DestinationConnectionId { get; set; }
        public byte SCID { get; set; }
        public byte SourceConnectionId { get; set; }
        public VariableInteger TokenLength { get; set; }
        public byte[] Token { get; set; }
        public VariableInteger Length { get; set; }
        public GranularInteger PacketNumber { get; set; }

        public InitialPacket()
        {
            DCID = SCID = 1;
        }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            // Size of the packet PacketNumber is determined by the last 2 bits of the Type.
            int pnSize = (type & 0x03) + 1;

            Version = array.ReadUInt32();

            DCID = array.ReadByte();
            if (DCID > 0)
                DestinationConnectionId = array.ReadByte();

            SCID = array.ReadByte();
            if (SCID > 0)
                SourceConnectionId = array.ReadByte();

            TokenLength = array.ReadVariableInteger();
            if (TokenLength != 0)
                Token = array.ReadBytes((int)TokenLength.Value);

            Length = array.ReadVariableInteger();
            PacketNumber = array.ReadBytes(pnSize);

            Length = Length - PacketNumber.Size;

            this.DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();
            result.Add((byte)(Type | (PacketNumber.Size - 1)));
            result.AddRange(ByteUtilities.GetBytes(Version));

            result.Add(DCID);
            if (DCID > 0)
                result.Add(DestinationConnectionId);
            result.Add(SCID);
            if (SCID > 0)
                result.Add(SourceConnectionId);

            byte[] tokenLength = new VariableInteger(0);
            byte[] length = new VariableInteger(PacketNumber.Size + (UInt64)frames.Length);

            result.AddRange(tokenLength);
            result.AddRange(length);
            byte[] packetNumber = PacketNumber;
            result.AddRange(packetNumber);
            result.AddRange(frames);

            return result.ToArray();
        }
    }
}
