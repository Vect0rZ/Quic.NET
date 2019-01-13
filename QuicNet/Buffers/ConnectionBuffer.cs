using QuicNet.Infrastructure.Packets;
using QuicNet.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace QuicNet.Buffers
{
    public class ConnectionBuffer
    {
        private Queue<Packet> _queue;
        private Timer _expiry;

        public ConnectionBuffer()
        {
            _queue = new Queue<Packet>();
            _expiry = new Timer(QuicSettings.BufferTimeInverval);

            _expiry.Start();
        }

        public void Push(Packet packet)
        {
            _queue.Enqueue(packet);
        }

        public List<Packet> ReadAll()
        {
            List<Packet> result = new List<Packet>();
            result = _queue.ToList();

            return result;
        }
    }
}
