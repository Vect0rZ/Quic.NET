using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class StreamFrame : Frame
    {
        public byte ActualType = 0x08;

        public override byte Type => 0x08;
        public VariableInteger StreamId { get; set; }
        public VariableInteger Offset { get; set; }
        public VariableInteger Length { get; set; }
        public byte[] StreamData { get; set; }
        public bool EndOfStream { get; set; }

        public StreamFrame()
        {

        }

        public StreamFrame(UInt64 streamId, byte[] data, UInt64 offset, bool eos)
        {
            StreamId = streamId;
            StreamData = data;
            Offset = offset;
            Length = (UInt64)data.Length;
            EndOfStream = eos;
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();

            byte OFF_BIT = (byte)(type & 0x04);
            byte LEN_BIT = (byte)(type & 0x02);
            byte FIN_BIT = (byte)(type & 0x01);

            StreamId = array.ReadVariableInteger();
            if (OFF_BIT > 0)
                Offset = array.ReadVariableInteger();
            if (LEN_BIT > 0)
                Length = array.ReadVariableInteger();
            if (FIN_BIT > 0)
                EndOfStream = true;

            StreamData = array.ReadBytes((int)Length.Value);
        }

        public override byte[] Encode()
        {
            if (Offset != null && Offset.Value > 0)
                ActualType = (byte)(ActualType | 0x04);
            if (Length != null && Length.Value > 0)
                ActualType = (byte)(ActualType | 0x02);
            if (EndOfStream == true)
                ActualType = (byte)(ActualType | 0x01);

            byte OFF_BIT = (byte)(ActualType & 0x04);
            byte LEN_BIT = (byte)(ActualType & 0x02);
            byte FIN_BIT = (byte)(ActualType & 0x01);

            List<byte> result = new List<byte>();
            result.Add(ActualType);
            byte[] streamId = StreamId;
            result.AddRange(streamId);

            if (OFF_BIT > 0)
                result.AddRange(Offset.ToByteArray());

            if (LEN_BIT > 0)
                result.AddRange(Length.ToByteArray());

            result.AddRange(StreamData);

            return result.ToArray();
        }
    }
}
