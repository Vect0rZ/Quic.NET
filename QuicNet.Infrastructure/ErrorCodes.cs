using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuicNet.Infrastructure
{
    public enum ErrorCode : UInt16
    {
        NO_ERROR = 0x0,
        INTERNAL_ERROR = 0x1,
        CONNECTION_REFUSED = 0x2,
        FLOW_CONTROL_ERROR = 0x3,
        STREAM_LIMIT_ERROR = 0x4,
        STREAM_STATE_ERROR = 0x5,
        FINAL_SIZE_ERROR = 0x6,
        FRAME_ENCODING_ERROR = 0x7,
        TRANSPORT_PARAMETER_ERROR = 0x8,
        CONNECTION_ID_LIMIT_ERROR = 0x9,
        PROTOCOL_VIOLATION = 0xA,
        INVALID_TOKEN = 0xB,
        APPLICATION_ERROR = 0xC,
        CRYPTO_BUFFER_EXCEEDED = 0xD,
        KEY_UPDATE_ERROR = 0xE,
        AEAD_LIMIT_REACHED = 0xF,
        CRYPTO_ERROR = 0x100
    }
}
