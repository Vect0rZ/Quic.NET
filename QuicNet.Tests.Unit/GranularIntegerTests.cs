using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuickNet.Utilities;

namespace QuicNet.Tests.Unit
{
    [TestClass]
    public class GranularIntegerTests
    {
        [TestMethod]
        public void Zero()
        {
            GranularInteger integer = new GranularInteger(0);
            byte[] bin = integer;
            UInt64 num = integer;

            Assert.IsNotNull(bin);
            Assert.AreEqual(bin.Length, 1);
            Assert.AreEqual(bin[0], (byte)0);
            Assert.AreEqual(num, (UInt64)0);
        }

        [TestMethod]
        public void One()
        {
            GranularInteger integer = new GranularInteger(1);
            byte[] bin = integer;
            UInt64 num = integer;

            Assert.IsNotNull(bin);
            Assert.AreEqual(bin.Length, 1);
            Assert.AreEqual(bin[0], (byte)1);
            Assert.AreEqual(num, (UInt64)1);
        }

        [TestMethod]
        public void Test255()
        {
            GranularInteger integer = new GranularInteger(255);
            byte[] bin = integer;
            UInt64 num = integer;

            Assert.IsNotNull(bin);
            Assert.AreEqual(bin.Length, 1);
            Assert.AreEqual(bin[0], (byte)255);
            Assert.AreEqual(num, (UInt64)255);
        }

        [TestMethod]
        public void Test256()
        {
            GranularInteger integer = new GranularInteger(256);
            byte[] bin = integer;
            UInt64 num = integer;

            Assert.IsNotNull(bin);
            Assert.AreEqual(bin.Length, 2);
            Assert.AreEqual(bin[0], (byte)1);
            Assert.AreEqual(bin[1], (byte)0);
            Assert.AreEqual(num, (UInt64)256);
        }

        [TestMethod]
        public void TestGranularIntegerMaxValue()
        {
            GranularInteger integer = new GranularInteger(GranularInteger.MaxValue);
            byte[] bin = integer;
            UInt64 num = integer;

            Assert.IsNotNull(bin);
            Assert.AreEqual(bin.Length, 8);
            Assert.AreEqual(num, GranularInteger.MaxValue);
        }
    }
}
