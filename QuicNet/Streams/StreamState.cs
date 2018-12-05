using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Streams
{
    public enum StreamState
    {
        Recvd,
        SizeKnown,
        DataRecvd,
        DataRead,
        ResetRecvd
    }
}
