namespace WindowStreamer.Protocol;

public static class Constants
{
    /// <summary>
    /// The constant length of a meta-packet.
    /// </summary>
    public static readonly int MetaFrameLength = 30;
    public static readonly char ParameterSeparator = ',';
    public static readonly char SingleSeparator = '.';

    // Meta connection messages:
    // Handshakes: status code(1 = Accept, 0 = Deny)
    // {message type}, {status code}
    // {message type}, {status code}, {video resolution}, {}
}
