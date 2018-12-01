using QuickNet.Utilities;
using QuicNet.Infrastructure.Frames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public class FrameFactory
    {
        private ByteArray _array;

        public FrameFactory(ByteArray array)
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
                case 0x02: result = new ConnectionCloseFrame(); break;
                default: result = null; break;
            }

            if (result != null)
                result.Decode(_array);

            return result;
        }
    }
}
