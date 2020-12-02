using QuicNet.Connections;
using QuicNet.Context;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Tests.ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Example();
            await ExampleAsync();
            Console.ReadKey();
        }

        private static void Example()
        {
            Console.WriteLine("Starting client.");
            QuicClient client = new QuicClient();
            Console.WriteLine("Connecting to server.");
            QuicConnection connection = client.Connect("127.0.0.1", 11000);   // Connect to peer (Server)
            Console.WriteLine("Connected");

            QuicStream stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional); // Create a data stream
            Console.WriteLine("Create stream with id: " + stream.StreamId.IntegerValue.ToString());

            Console.WriteLine("Send 'Hello From Client!'");
            stream.Send(Encoding.UTF8.GetBytes("Hello from Client!"));        // Send Data

            stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional); // Create a data stream
            stream.Send(Encoding.UTF8.GetBytes("Hello from Client2!"));

            Console.WriteLine("Waiting for message from the server");
            try
            {
                byte[] data = stream.Receive();                                   // Receive from server
                Console.WriteLine("Received: " + Encoding.UTF8.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                byte[] data = stream.Receive();                                   // Receive from server
                Console.WriteLine("Received: " + Encoding.UTF8.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task ExampleAsync()
        {
            Console.WriteLine("Starting client.");
            QuicClient client = new QuicClient();
            Console.WriteLine("Connecting to server.");
            QuicConnection connection = await client.ConnectAsync("127.0.0.1", 11000);   // Connect to peer (Server)
            Console.WriteLine("Connected");

            QuicStream stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional); // Create a data stream
            Console.WriteLine("Create stream with id: " + stream.StreamId.IntegerValue.ToString());

            Console.WriteLine("Send 'Hello From Client!'");
            await stream.SendAsync(Encoding.UTF8.GetBytes("Hello from Client!"));        // Send Data

            stream = connection.CreateStream(QuickNet.Utilities.StreamType.ClientBidirectional); // Create a data stream
            await stream.SendAsync(Encoding.UTF8.GetBytes("Hello from Client2!"));

            Console.WriteLine("Waiting for message from the server");
            try
            {
                byte[] data = await stream.ReceiveAsync();                                   // Receive from server
                Console.WriteLine("Received: " + Encoding.UTF8.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            try
            {
                byte[] data = await stream.ReceiveAsync();                                   // Receive from server
                Console.WriteLine("Received: " + Encoding.UTF8.GetString(data));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
