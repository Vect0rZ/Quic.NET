using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Tests.Unit
{
    [TestClass]
    public class StreamIdTests
    {
        [TestMethod]
        public void ClientBidirectional()
        {
            StreamId id = new StreamId(123, StreamType.ClientBidirectional);
            byte[] data = id;

            Assert.IsNotNull(data);
            Assert.AreEqual(data.Length, 8);
            Assert.AreEqual(data[6], 1);
            Assert.AreEqual(data[7], 236);
        }

        [TestMethod]
        public void ClientUnidirectional()
        {
            StreamId id = new StreamId(123, StreamType.ClientUnidirectional);
            byte[] data = id;

            Assert.IsNotNull(data);
            Assert.AreEqual(data.Length, 8);
            Assert.AreEqual(data[6], 1);
            Assert.AreEqual(data[7], 238);
        }

        [TestMethod]
        public void ServerBidirectional()
        {
            StreamId id = new StreamId(123, StreamType.ServerBidirectional);
            byte[] data = id;

            Assert.IsNotNull(data);
            Assert.AreEqual(data.Length, 8);
            Assert.AreEqual(data[6], 1);
            Assert.AreEqual(data[7], 237);
        }

        [TestMethod]
        public void ServerUnidirectional()
        {
            StreamId id = new StreamId(123, StreamType.ServerUnidirectional);
            byte[] data = id;

            Assert.IsNotNull(data);
            Assert.AreEqual(data.Length, 8);
            Assert.AreEqual(data[6], 1);
            Assert.AreEqual(data[7], 239);
        }

        [TestMethod]
        public void VariableIntegerTest()
        {
            StreamId id = new StreamId(123, StreamType.ClientBidirectional);

            VariableInteger integer = id;

            StreamId converted = integer;

            Assert.AreEqual(id.Id, converted.Id);
        }
    }
}
