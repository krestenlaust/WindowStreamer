namespace Protocol
{
    public static class Constants
    {
        public const int MetaFrameLength = 30;
        public const char ParameterSeparator = ',';
        public const char SingleSeparator = '.';

        //Meta connection messages
        // Handshakes: status code(1 = Accept, 0 = Deny)
        // {message type}, {status code}
        // {message type}, {status code}, {video resolution}, {}
    }
}
