using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class InitialPacket : Packet
    {
        public override byte Type => 0b1100_1100 ; //0xDC; // 1101 1100
        
        public byte DestinationConnectionIdLength { get; set; }
        public GranularInteger DestinationConnectionId { get; set; }
        public byte SourceConnectionIdLength { get; set; }
        public GranularInteger SourceConnectionId { get; set; }
        public VariableInteger TokenLength { get; set; }
        public byte[] Token { get; set; }
        public VariableInteger Length { get; set; }
        public GranularInteger PacketNumber { get; set; }

        public InitialPacket()
        {

        }

        public InitialPacket(GranularInteger destinationConnectionId, GranularInteger sourceConnectionId)
        {
            DestinationConnectionIdLength = destinationConnectionId.Size;
            DestinationConnectionId = destinationConnectionId;

            SourceConnectionIdLength = sourceConnectionId.Size;
            SourceConnectionId = sourceConnectionId;
        }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            // Size of the packet PacketNumber is determined by the last 2 bits of the Type.
            int pnSize = (type & 0x03) + 1;

            Version = array.ReadUInt32();

            DestinationConnectionIdLength = array.ReadByte();
            if (DestinationConnectionIdLength > 0)
                DestinationConnectionId = array.ReadGranularInteger(DestinationConnectionIdLength);

            SourceConnectionIdLength = array.ReadByte();
            if (SourceConnectionIdLength > 0)
                SourceConnectionId = array.ReadGranularInteger(SourceConnectionIdLength);

            TokenLength = array.ReadVariableInteger();
            if (TokenLength > 0)
                Token = array.ReadBytes(TokenLength);

            Length = array.ReadVariableInteger();
            PacketNumber = array.ReadGranularInteger(pnSize);

            Length = Length - PacketNumber.Size;

            this.DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();
            result.Add((byte)(Type | (PacketNumber.Size - 1)));
            result.AddRange(ByteUtilities.GetBytes(Version));

            result.Add(DestinationConnectionId.Size);
            if (DestinationConnectionId.Size > 0)
                result.AddRange(DestinationConnectionId.ToByteArray());
            result.Add(SourceConnectionId.Size);
            if (SourceConnectionId.Size > 0)
                result.AddRange(SourceConnectionId.ToByteArray());

            // TODO: Implement Token
            byte[] tokenLength = new VariableInteger(0);
            byte[] length = new VariableInteger(PacketNumber.Size + (UInt64)frames.Length);

            result.AddRange(tokenLength);
            result.AddRange(length);
            result.AddRange(PacketNumber.ToByteArray());
            result.AddRange(frames);

            return result.ToArray();
        }
    }
}
