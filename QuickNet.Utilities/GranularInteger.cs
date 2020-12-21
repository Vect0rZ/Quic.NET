using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Utilities
{
    public class GranularInteger
    {
        public const UInt64 MaxValue = 18446744073709551615;

        private UInt64 _integer;
        public UInt64 Value { get { return _integer; } }
        public byte Size { get { return RequiredBytes(Value); } }

        public GranularInteger(UInt64 integer)
        {
            _integer = integer;
        }

        public byte[] ToByteArray()
        {
            return Encode(this._integer);
        }

        public static implicit operator byte[](GranularInteger integer)
        {
            return Encode(integer._integer);
        }

        public static implicit operator GranularInteger(byte[] bytes)
        {
            return new GranularInteger(Decode(bytes));
        }

        public static implicit operator GranularInteger(UInt64 integer)
        {
            return new GranularInteger(integer);
        }

        public static implicit operator UInt64(GranularInteger integer)
        {
            return integer._integer;
        }

        public static byte[] Encode(UInt64 integer)
        {
            byte requiredBytes = RequiredBytes(integer);
            int offset = 8 - requiredBytes;

            byte[] uInt64Bytes = ByteUtilities.GetBytes(integer);

            byte[] result = new byte[requiredBytes];
            Buffer.BlockCopy(uInt64Bytes, offset, result, 0, requiredBytes);

            return result;
        }

        public static UInt64 Decode(byte[] bytes)
        {
            int i = 8 - bytes.Length;
            byte[] buffer = new byte[8];

            Buffer.BlockCopy(bytes, 0, buffer, i, bytes.Length);

            UInt64 res = ByteUtilities.ToUInt64(buffer);

            return res;
        }

        private static byte RequiredBytes(UInt64 integer)
        {
            byte result = 0;

            if (integer <= byte.MaxValue) /* 255 */
                result = 1;
            else if (integer <= UInt16.MaxValue) /* 65535 */
                result = 2;
            else if (integer <= UInt32.MaxValue) /* 4294967295 */
                result = 4;
            else if (integer <= UInt64.MaxValue) /* 18446744073709551615 */
                result = 8;
            else
                throw new ArgumentOutOfRangeException("Value is larger than GranularInteger.MaxValue.");

            return result;
        }
    }
}
