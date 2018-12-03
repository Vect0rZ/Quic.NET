using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Utilities
{
    public class ByteArray
    {
        private int _length;
        private int _offset;
        private bool _readable;
        private byte[] _array;

        public ByteArray(byte[] array)
        {
            _readable = true;
            _array = array;

            if (array == null || array.Length <= 0)
            {
                _readable = false;
            }
            else
            {
                _length = array.Length;
                _offset = 0;
            }
        }

        public byte ReadByte()
        {
            byte result = _array[_offset++];
            return result;
        }

        public byte PeekByte()
        {
            byte result = _array[_offset];
            return result;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];
            Buffer.BlockCopy(_array, _offset, bytes, 0, count);

            _offset += count;

            return bytes;
        }

        public UInt16 ReadUInt16()
        {
            byte[] bytes = ReadBytes(2);
            UInt16 result = ByteUtilities.ToUInt16(bytes);

            return result;
        }

        public UInt32 ReadUInt32()
        {
            byte[] bytes = ReadBytes(4);
            UInt32 result = ByteUtilities.ToUInt32(bytes);

            return result;
        }

        public VariableInteger ReadVariableInteger()
        {
            // Set Token Length and Token
            byte initial = PeekByte();
            int size = VariableInteger.Size(initial);

            byte[] bytes = new byte[size];
            Buffer.BlockCopy(_array, _offset, bytes, 0, size);
            _offset += size;

            return bytes;
        }

        public StreamId ReadStreamId()
        {
            byte[] streamId = ReadBytes(8);
            StreamId result = streamId;

            return result;
        }

        public bool HasData()
        {
            return _offset < _length;
        }
    }
}
