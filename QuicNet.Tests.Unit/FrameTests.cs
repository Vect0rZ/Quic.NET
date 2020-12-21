using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickNet.Utilities;
using QuicNet.Constants;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Frames;

namespace QuicNet.Tests.Unit
{
    [TestClass]
    public class FrameTests
    {
        [TestMethod]
        public void ConnectionCloseFrameTest()
        {
            var ccf = new ConnectionCloseFrame(ErrorCode.PROTOCOL_VIOLATION, 0x00, ErrorConstants.PMTUNotReached);
            byte[] data = ccf.Encode();

            var nccf = new ConnectionCloseFrame();
            nccf.Decode(new ByteArray(data));

            Assert.AreEqual(ccf.ActualType, nccf.ActualType);
            Assert.AreEqual(ccf.FrameType.Value, nccf.FrameType.Value);
            Assert.AreEqual(ccf.ReasonPhraseLength.Value, nccf.ReasonPhraseLength.Value);
            Assert.AreEqual(ccf.ReasonPhrase, nccf.ReasonPhrase);
        }
    }
}
