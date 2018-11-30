using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Utilities
{
    public static class ByteUtilities
    {
        public static byte[] GetBytes(UInt64 integer)
        {
            byte[] result = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        public static byte[] GetBytes(UInt32 integer)
        {
            byte[] result = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        public static byte[] GetBytes(UInt16 integer)
        {
            byte[] result = BitConverter.GetBytes(integer);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] result = Encoding.UTF8.GetBytes(str);

            return result;
        }

        public static UInt64 ToUInt64(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            UInt64 result = BitConverter.ToUInt64(data, 0);

            return result;
        }

        public static UInt32 ToUInt32(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            UInt32 result = BitConverter.ToUInt32(data, 0);

            return result;
        }

        public static UInt16 ToUInt16(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(data);

            UInt16 result = BitConverter.ToUInt16(data, 0);

            return result;
        }

        public static string GetString(byte[] str)
        {
            string result = Encoding.UTF8.GetString(str);

            return result;
        }

    }
}
