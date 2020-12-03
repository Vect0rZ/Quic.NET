using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Constants
{
    public class ErrorConstants
    {
        public const string ServerTooBusy = "The server is too busy to process your request.";
        public const string MaxDataTransfer = "Maximum data transfer reached.";
        public const string MaxNumberOfStreams = "Maximum number of streams reached.";
        public const string PMTUNotReached = "PMTU have not been reached.";
    }
}
