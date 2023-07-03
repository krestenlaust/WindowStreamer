namespace WindowStreamer.Server;

public enum ConnectionReply
{
    /// <summary>
    /// Accepts connection, and initiates handshake.
    /// </summary>
    Accept,

    /// <summary>
    /// Closes connection without further notice.
    /// </summary>
    Close,

    /// <summary>
    /// Responds to connection, then closes it.
    /// </summary>
    Deny,
}
