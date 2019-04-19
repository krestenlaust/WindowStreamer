using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    class Constants
    {
        public const int VideoStreamPort = 10064;
        public const int MetaStreamPort = 10063;
        public const int MetaFrameLength = 30;
        public const float FramerateCap = 10f;
        public const char ParameterSeparator = ',';
        public const char SingleSeparator = '.';

        //Meta connection messages
        // Handshakes: status code(1 = Accept, 0 = Deny)
        // {message type}, {status code}
        // {message type}, {status code}, {video resolution}, {}
    }
}
