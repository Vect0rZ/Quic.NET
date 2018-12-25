using QuickNet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure.Frames
{
    public class ConnectionCloseFrame : Frame
    {
        public override byte Type => 0x1c;
        public UInt16 ErrorCode { get; set; }
        public VariableInteger ReasonPhraseLength { get; set; }
        public string ReasonPhrase { get; set; }

        public ConnectionCloseFrame()
        {
            ErrorCode = 0;
            ReasonPhraseLength = new VariableInteger(0);
        }

        public ConnectionCloseFrame(ErrorCode error, string reason)
        {
            ReasonPhraseLength = new VariableInteger(0);

            ErrorCode = (UInt16)error;
            ReasonPhrase = reason;
        }

        public override void Decode(ByteArray array)
        {
            byte type = array.ReadByte();

            ErrorCode = array.ReadUInt16();
            ReasonPhraseLength = array.ReadVariableInteger();

            byte[] rp = array.ReadBytes((int)ReasonPhraseLength.Value);
            ReasonPhrase = ByteUtilities.GetString(rp);
        }

        public override byte[] Encode()
        {
            List<byte> result = new List<byte>();
            result.Add(Type);

            byte[] errorCode = ByteUtilities.GetBytes(ErrorCode);
            result.AddRange(errorCode);

            if (string.IsNullOrWhiteSpace(ReasonPhrase) == false)
            {
                byte[] reasonPhrase = ByteUtilities.GetBytes(ReasonPhrase);
                byte[] rpl = new VariableInteger((UInt64)ReasonPhrase.Length);
                result.AddRange(rpl);
                result.AddRange(reasonPhrase);
            }

            return result.ToArray();
        }
    }
}
