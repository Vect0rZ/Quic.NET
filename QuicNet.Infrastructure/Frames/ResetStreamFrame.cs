using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class ResetStreamFrame : Frame
    {
        public override byte Type => 0x04;
        public VariableInteger StreamId { get; set; }
        public VariableInteger ApplicationProtocolErrorCode { get; set; }
        public VariableInteger FinalSize { get; set; }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();
            StreamId = array.ReadVariableInteger();
            ApplicationProtocolErrorCode = array.ReadVariableInteger();
            FinalSize = array.ReadVariableInteger();
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();

            result.Add(Type);
            result.AddRange(StreamId.ToByteArray());
            result.AddRange(ApplicationProtocolErrorCode.ToByteArray());
            result.AddRange(FinalSize.ToByteArray());

            return result.ToArray();
        }
    }
}
