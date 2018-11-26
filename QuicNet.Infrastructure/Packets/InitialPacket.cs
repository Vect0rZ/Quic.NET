using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class InitialPacket : Packet
    {
        public override byte Type => 0x7F;
        public UInt32 Version { get; set; }
        public byte DCIL_SCIL { get; set; }
        public byte DestinationConnectionId { get; set; }
        public byte SourceConnectionId { get; set; }
        public VariableInteger TokenLength { get; set; }
        public byte[] Token { get; set; }
        public VariableInteger Length { get; set; }
        public UInt32 PacketNumber { get; set; }
        public byte[] Payload { get; set; }

        public InitialPacket()
        {
            DCIL_SCIL = 0;
        }

        public override void Decode(byte[] packet)
        {
            int offset = 1;

            // Set version
            byte[] version = new byte[4];
            Buffer.BlockCopy(packet, offset, version, 0, 4);
            Version = ByteUtilities.ToUInt32(version);
            offset += 4;

            // Set DCI and SCI
            DCIL_SCIL = packet[offset++];
            if ((DCIL_SCIL & 0xF0) != 0)
                DestinationConnectionId = packet[offset++];
            if ((DCIL_SCIL & 0x0F) != 0)
                SourceConnectionId = packet[offset++];

            // Set Token Length and Token
            int varIntSize = VariableInteger.Size(packet[offset]);
            byte[] tokenLength = new byte[varIntSize];
            Buffer.BlockCopy(packet, offset, tokenLength, 0, varIntSize);
            offset += varIntSize;
            TokenLength = tokenLength;

            if (TokenLength != 0)
                Buffer.BlockCopy(packet, offset += (int)TokenLength.Value, Token, 0, (int)TokenLength.Value);

            // Set Length
            varIntSize = VariableInteger.Size(packet[offset]);
            byte[] length = new byte[varIntSize];
            Buffer.BlockCopy(packet, offset, length, 0, varIntSize);
            offset += varIntSize;
            Length = length;

            // Set packet number
            byte[] packetNumber = new byte[4];
            Buffer.BlockCopy(packet, offset, packetNumber, 0, 4);
            PacketNumber = ByteUtilities.ToUInt32(packetNumber);

            Length = Length - 4;
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();
            result.Add(Type);
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
            byte[] length = new VariableInteger(4);

            result.AddRange(tokenLength);
            result.AddRange(length);
            result.AddRange(ByteUtilities.GetBytes(PacketNumber));

            return result.ToArray();
        }
    }
}
