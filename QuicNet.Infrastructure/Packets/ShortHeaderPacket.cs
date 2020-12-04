using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    public class ShortHeaderPacket : Packet
    {
        public byte ActualType = 0b01000000;
        public override byte Type => 0b01000000;

        public byte DestinationConnectionId { get; set; }
        public GranularInteger PacketNumber { get; set; }

        public override void Decode(byte[] packet)
        {
            ByteArray array = new ByteArray(packet);
            byte type = array.ReadByte();
            DestinationConnectionId = array.ReadByte();

            int pnSize = (type & 0x03) + 1;
            PacketNumber = array.ReadBytes(pnSize);

            DecodeFrames(array);
        }

        public override byte[] Encode()
        {
            byte[] frames = EncodeFrames();
           
            List<byte> result = new List<byte>();
            result.Add((byte)(Type | (PacketNumber.Size - 1)));
            result.Add(DestinationConnectionId);

            byte[] pnBytes = PacketNumber;
            result.AddRange(pnBytes);
            result.AddRange(frames);

            return result.ToArray();
        }
    }
}
