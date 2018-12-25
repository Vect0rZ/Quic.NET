using QuickNet.Utilities;
using QuicNet.Infrastructure.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public class FrameParser
    {
        private ByteArray _array;

        public FrameParser(ByteArray array)
        {
            _array = array;
        }

        public Frame GetFrame()
        {
            Frame result;
            byte frameType = _array.PeekByte();
            switch(frameType)
            {
                case 0x00: result = new PaddingFrame(); break;
                case 0x01: result = new PingFrame(); break;
                case 0x02: result = new AckFrame(); break;
                case 0x03: result = new AckFrame(); break;
                case 0x04: result = new ResetStreamFrame(); break;
                case 0x05: result = new StopSendingFrame(); break;
                case 0x06: result = new CryptoFrame(); break;
                case 0x07: result = new NewTokenFrame(); break;
                case 0x08: result = new StreamFrame(); break;
                case 0x09: result = new StreamFrame(); break;
                case 0x0a: result = new StreamFrame(); break;
                case 0x0b: result = new StreamFrame(); break;
                case 0x0c: result = new StreamFrame(); break;
                case 0x0d: result = new StreamFrame(); break;
                case 0x0e: result = new StreamFrame(); break;
                case 0x0f: result = new StreamFrame(); break;
                case 0x10: result = new MaxDataFrame(); break;
                case 0x11: result = new MaxStreamDataFrame(); break;
                case 0x12: result = new MaxStreamsFrame(); break;
                case 0x13: result = new MaxStreamsFrame(); break;
                case 0x14: result = new DataBlockedFrame(); break;
                case 0x15: result = new StreamDataBlockedFrame(); break;
                case 0x16: result = new StreamsBlockedFrame(); break;
                case 0x17: result = new StreamsBlockedFrame(); break;
                case 0x18: result = new NewConnectionIdFrame(); break;
                case 0x19: result = new RetireConnectionIdFrame(); break;
                case 0x1a: result = new PathChallengeFrame(); break;
                case 0x1b: result = new PathResponseFrame(); break;
                case 0x1c: result = new ConnectionCloseFrame(); break;
                case 0x1d: result = new ConnectionCloseFrame(); break;
                default: result = null; break;
            }

            if (result != null)
                result.Decode(_array);

            return result;
        }
    }
}
