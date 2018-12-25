using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public enum QuicPacketType
    {
        Initial,
        LongHeader,
        ShortHeader,
        VersionNegotiation,
        Broken
    }
}
