using QuickNet.Utilities;
using QuicNet;
using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Infrastructure;
using QuicNet.Infrastructure.Frames;
using QuicNet.Infrastructure.PacketProcessing;
using QuicNet.Infrastructure.Packets;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNet.Tests.ConsoleServer
{
    class Program
    {
        /// <summary>
        /// Fired when Client is connected
        /// </summary>
        /// <param name="connection">The new connection</param>
        static void ClientConnected(QuicConnection connection)
        {
            Console.WriteLine("Client Connected");
            connection.OnStreamOpened += StreamOpened;
        }

        static void StreamOpened(QuicStream stream)
        {
            Console.WriteLine("Stream Opened");
            stream.OnStreamDataReceived += StreamDataReceived;
        }

        static void StreamDataReceived(QuicStream stream, byte[] data)
        {
            Console.WriteLine("Stream Data Received: ");
            string decoded = Encoding.UTF8.GetString(data);
            Console.WriteLine(decoded);

            stream.Send(Encoding.UTF8.GetBytes("Ping back from server."));
        }

        static void Example()
        {
            QuicListener listener = new QuicListener(11000);
            listener.OnClientConnected += ClientConnected;

            listener.Start();

            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Example();
            return;
        }
    }
}
