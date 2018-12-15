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
                case 0x01: result = new RSTStreamFrame(); break;
                case 0x02: result = new ConnectionCloseFrame(); break;
                case 0x06: result = new MaxStreamIdFrame(); break;
                case 0x10: result = new StreamFrame(); break;
                case 0x11: result = new StreamFrame(); break;
                case 0x12: result = new StreamFrame(); break;
                case 0x13: result = new StreamFrame(); break;
                case 0x14: result = new StreamFrame(); break;
                case 0x15: result = new StreamFrame(); break;
                case 0x16: result = new StreamFrame(); break;
                case 0x17: result = new StreamFrame(); break;
                default: result = null; break;
            }

            if (result != null)
                result.Decode(_array);

            return result;
        }
    }
}
