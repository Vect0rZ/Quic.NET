using QuickNet.Utilities;
using QuicNet.Infrastructure.Frames;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Packets
{
    /// <summary>
    /// Base data transfer unit of QUIC Transport.
    /// </summary>
    public abstract class Packet
    {
        protected List<Frame> _frames = new List<Frame>();
        public abstract byte Type { get; }

        public UInt32 Version { get; set; }

        public abstract byte[] Encode();
        public abstract void Decode(byte[] packet);

        public virtual void AttachFrame(Frame frame)
        {
            _frames.Add(frame);
        }

        public virtual void DecodeFrames(ByteArray array)
        {
            FrameFactory factory = new FrameFactory(array);
            Frame result;
            while (array.HasData())
            {
                result = factory.GetFrame();
                if (result == null)
                {
                    // TODO: Handle broken frames.
                }
                _frames.Add(result);
            }
        }

        public virtual byte[] EncodeFrames()
        {
            List<byte> result = new List<byte>();
            foreach (Frame frame in _frames)
            {
                result.AddRange(frame.Encode());
            }

            return result.ToArray();
        }
    }
}
