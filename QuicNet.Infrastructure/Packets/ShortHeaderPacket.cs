using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class ShortHeaderPacket : Packet
    {
        public byte ActualType = 0b0100_0000;
        public override byte Type => 0b0100_0000;

        public GranularInteger DestinationConnectionId { get; set; }
        public GranularInteger PacketNumber { get; set; }

        // Field not transferred! Only the connection knows about the length of the ConnectionId
        public byte DestinationConnectionIdLength { get; set; }

        public ShortHeaderPacket(byte destinationConnectionIdLength)
        {
            DestinationConnectionIdLength = destinationConnectionIdLength;
        }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            DestinationConnectionId = array.ReadGranularInteger(DestinationConnectionIdLength);

            int pnSize = (type & 0x03) + 1;
            PacketNumber = array.ReadBytes(pnSize);

            DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();

            List<byte> result = new List<byte>();
            result.Add((byte)(Type | (PacketNumber.Size - 1)));
            result.AddRange(DestinationConnectionId.ToByteArray());

            byte[] pnBytes = PacketNumber;
            result.AddRange(pnBytes);
            result.AddRange(frames);

            return result.ToArray();
        }
    }
}
