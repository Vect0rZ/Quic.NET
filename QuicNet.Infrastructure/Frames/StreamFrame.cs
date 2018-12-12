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
        private byte ActualType = 0x10;

        public override byte Type => 0x10;
        public VariableInteger StreamId { get; set; }
        public VariableInteger Offset { get; set; }
        public VariableInteger Length { get; set; }
        public byte[] StreamData { get; set; }
        public StreamId ConvertedStreamId { get; set; }
        public bool EndOfStream { get; set; }

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
            ConvertedStreamId = QuickNet.Utilities.StreamId.Decode(ByteUtilities.GetBytes(Length.Value));
        }

        public override byte[] Encode()
        {
            byte OFF_BIT = (byte)(ActualType & 0x04);
            byte LEN_BIT = (byte)(ActualType & 0x02);
            byte FIN_BIT = (byte)(ActualType & 0x01);

            List<byte> result = new List<byte>();
            result.Add(ActualType);
            byte[] streamId = StreamId;
            result.AddRange(streamId);

            if (OFF_BIT > 0)
            {
                byte[] offset = Offset;
                result.AddRange(offset);
            }

            if (LEN_BIT > 0)
            {
                byte[] length = Length;
                result.AddRange(length);
            }

            result.AddRange(StreamData);

            return result.ToArray();
        }

        public void SetFlags(bool off, bool len, bool fin)
        {
            ActualType = off ? (byte)(ActualType & 0x04) : ActualType;
            ActualType = len ? (byte)(ActualType & 0x02) : ActualType;
            ActualType = fin ? (byte)(ActualType & 0x01) : ActualType;
        }
    }
}
