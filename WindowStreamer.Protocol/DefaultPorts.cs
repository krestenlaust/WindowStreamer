namespace WindowStreamer.Protocol;

/// <summary>
/// Contains definitions for the default ports to be used in the application.
/// </summary>
public static class DefaultPorts
{
    /// <summary>
    /// Default port to be used for the TCP socket in communicating meta-information.
    /// </summary>
    public static readonly int MetastreamPort = 10063;

    /// <summary>
    /// Default port to be used for the UDP client for sending and receiving datagrams of videodata.
    /// </summary>
    public static readonly int VideostreamPort = 10064;
}
