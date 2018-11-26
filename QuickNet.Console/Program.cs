using QuickNet.Utilities;
using QuicNet;
using QuicNet.Infrastructure.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = new VariableInteger(12345);
            VariableInteger integer = bytes;
            UInt64 uinteger = integer;
            int size = VariableInteger.Size(bytes[0]);

            InitialPacket packet = new InitialPacket()
            {
                Version = 16,
                SourceConnectionId = 124,
                DestinationConnectionId = 0,
                PacketNumber = 777521,
                TokenLength = 0
            };

            byte[] data = packet.Encode();
            packet.Decode(data);
        }
    }
}
