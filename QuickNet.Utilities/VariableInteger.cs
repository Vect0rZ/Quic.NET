using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Utilities
{
    public class VariableInteger
    {
        private UInt64 _integer;
        public VariableInteger(UInt64 integer)
        {
            _integer = integer;
        }

        public static implicit operator byte[](VariableInteger integer)
        {
            return Encode(integer._integer);
        }

        public static implicit operator VariableInteger(byte[] bytes)
        {
            return new VariableInteger(Decode(bytes));
        }

        public static implicit operator UInt64(VariableInteger integer)
        {
            return integer._integer;
        }

        public static int Size(byte firstByte)
        {
            int result = (int)Math.Pow(2, (firstByte >> 6));

            return result;
        }

        public static byte[] Encode(UInt64 integer)
        {
            int requiredBytes = 0;
            if (integer <= byte.MaxValue >> 2) /* 63 */
                requiredBytes = 1;
            else if (integer <= UInt16.MaxValue >> 2) /* 16383 */
                requiredBytes = 2;
            else if (integer <= UInt32.MaxValue >> 2) /* 1073741823 */
                requiredBytes = 4;
            else if (integer <= UInt64.MaxValue >> 2) /* 4611686018427387903 */
                requiredBytes = 8;

            byte[] uInt64Bytes = BitConverter.GetBytes(integer);
            byte last = uInt64Bytes[requiredBytes - 1];
            last = (byte)(last | (requiredBytes / 2) << 6);
            uInt64Bytes[requiredBytes - 1] = last;

            byte[] result = new byte[requiredBytes];
            Buffer.BlockCopy(uInt64Bytes, 0, result, 0, requiredBytes);

            return result;
        }

        public static UInt64 Decode(byte[] bytes)
        {
            int i = bytes.Length - 1;
            byte[] buffer = new byte[8];

            Buffer.BlockCopy(bytes, 0, buffer, 0, bytes.Length);
            buffer[i] = (byte)(buffer[i] & (255 >> 2));

            UInt64 res = BitConverter.ToUInt64(buffer, 0);

            return res;
        }
    }
}
