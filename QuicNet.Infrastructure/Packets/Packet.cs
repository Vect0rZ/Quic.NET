using QuickNet.Utilities;
using QuicNet.Infrastructure.Exceptions;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.Settings;
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

        public virtual List<Frame> GetFrames()
        {
            return _frames;
        }

        public virtual void DecodeFrames(ByteArray array)
        {
            FrameParser factory = new FrameParser(array);
            Frame result;
            int frames = 0;
            while (array.HasData() && frames <= QuicSettings.PMTU)
            {
                result = factory.GetFrame();
                if (result != null)
                    _frames.Add(result);

                frames++;

                // TODO: Possibily handle broken frames.
            }

            if (array.HasData())
                throw new ProtocolException("Unexpected number of frames or possibly corrupted frame was sent.");
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
