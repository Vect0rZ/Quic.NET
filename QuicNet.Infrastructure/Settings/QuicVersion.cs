using System;
using System.Collections.Generic;
using System.Text;

namespace QuicNet.Infrastructure.Settings
{
    public class QuicVersion
    {
        public const int CurrentVersion = 16;

        public static readonly List<int> SupportedVersions = new List<int>() { 15, 16 };
    }
}
