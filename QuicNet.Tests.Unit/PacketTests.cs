using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Packets;

namespace QuicNet.Tests.Unit
{
    [TestClass]
    public class PacketTests
    {
        [TestMethod]
        public void LongeHeaderPacketTest()
        {
            LongHeaderPacket packet = new LongHeaderPacket(Infrastructure.PacketType.Handshake, 123415332, 1);
            packet.Version = 32;

            for (int i = 0; i < 123; i++)
            {
                packet.AttachFrame(new PaddingFrame());
            }

            byte[] data = packet.Encode();

            LongHeaderPacket result = new LongHeaderPacket();
            result.Decode(data);

            Assert.AreEqual(packet.Type, result.Type);
            Assert.AreEqual(packet.Version, result.Version);
            Assert.AreEqual(packet.DestinationConnectionIdLength, result.DestinationConnectionIdLength);
            Assert.AreEqual(packet.DestinationConnectionId.Value, result.DestinationConnectionId.Value);
            Assert.AreEqual(packet.SourceConnectionIdLength, result.SourceConnectionIdLength);
            Assert.AreEqual(packet.SourceConnectionId.Value, result.SourceConnectionId.Value);
            Assert.AreEqual(packet.PacketType, result.PacketType);
            Assert.AreEqual(packet.GetFrames().Count, result.GetFrames().Count);
        }
    }
}
