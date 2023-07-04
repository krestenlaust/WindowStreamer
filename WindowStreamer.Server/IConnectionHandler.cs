using System.Net;

namespace WindowStreamer.Server;

/// <summary>
/// Decides what connection attempts to drop and which to accept.
/// </summary>
public interface IConnectionHandler
{
    /// <summary>
    /// Recieves information about an incoming connection attempt, and returns whether to accept the connection.
    /// </summary>
    /// <param name="address">The IP-address of the host connecting to the device.</param>
    /// <returns>Whether to accept the connection.</returns>
    ConnectionReply HandleIncomingConnection(IPAddress address);
}
