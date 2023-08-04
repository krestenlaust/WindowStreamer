using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Serilog;
using WindowStreamer.Image;
using WindowStreamer.Protocol;

namespace WindowStreamer.Server;

/// <summary>
/// Represents a connected client. Handles all communication that is sent to this individual client.
/// </summary>
internal class ConnectedClient : IDisposable
{
    readonly CancellationTokenSource metastreamToken;
    bool connectionClosedInvoked;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectedClient"/> class.
    /// </summary>
    /// <param name="client">The tcp connection to associate with this client.</param>
    public ConnectedClient(TcpClient client)
    {
        this.NetworkClient = client;
        EndPoint = (IPEndPoint)client.Client.LocalEndPoint!;

        metastreamToken = new CancellationTokenSource();
        Task.Run(() => MetastreamLoop(client.GetStream()), metastreamToken.Token);
    }

    /// <summary>
    /// Called whenever an active connection has been closed.
    /// Is only called for connections that completed a handshake.
    /// </summary>
    public event Action? ConnectionClosed;

    /// <summary>
    /// Gets the endpoint of the this client.
    /// </summary>
    public IPEndPoint EndPoint { get; }

    /// <summary>
    /// Gets the endpoint target of UDP datagrams.
    /// </summary>
    public IPEndPoint UDPEndPoint => new IPEndPoint(EndPoint.Address, DefaultPorts.VideostreamPort);

    /// <summary>
    /// Gets the underlying tcp connection associated with this client.
    /// </summary>
    public TcpClient NetworkClient { get; }

    /// <summary>
    /// Gets a value indicating whether the client is ready to receive video frames.
    /// </summary>
    public bool UdpReady { get; private set; }

    /// <summary>
    /// Gets framerate to transmit captured images at, expressed in hertz.
    /// </summary>
    public int FramerateCapHz { get; private set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Halt server
        metastreamToken?.Cancel();

        // Dispose of networked instances
        NetworkClient?.Dispose();

        // Dispose of token sources
        metastreamToken?.Dispose();
    }

    /// <summary>
    /// Notifies client of resolution change.
    /// </summary>
    /// <param name="resolution">The new resolution.</param>
    public void UpdateResolution(ImageSize resolution)
    {
        // Notifies client of resolution change.
        if (NetworkClient is null || !NetworkClient.Connected)
        {
            Log.Debug("Not connected...");
            return;
        }

        var stream = NetworkClient.GetStream();
        new ServerMessage
        {
            ResolutionChange = new ResolutionChange
            {
                Width = resolution.Width,
                Height = resolution.Height,
            },
        }.WriteDelimitedTo(stream);
    }

    void MetastreamLoop(NetworkStream stream)
    {
        while (NetworkClient.Connected)
        {
            ClientMessage msg;
            try
            {
                msg = ClientMessage.Parser.ParseDelimitedFrom(stream);
            }
            catch (IOException)
            {
                // Client disconnected
                ClientDisconnected();
                return;
            }
            catch (OperationCanceledException)
            {
                // Server stopped
                ClientDisconnected();
                return;
            }

            switch (msg.MsgCase)
            {
                case ClientMessage.MsgOneofCase.None:
                    Log.Debug("Unknown message received");
                    break;
                case ClientMessage.MsgOneofCase.UDPReady:
                    Log.Debug("Udp ready!");
                    var udpReady = msg.UDPReady;

                    FramerateCapHz = udpReady.FramerateCap;
                    UdpReady = true;
                    break;
                default:
                    break;
            }
        }

        Log.Information("Connection lost... or disconnected");
    }

    void ClientDisconnected()
    {
        if (connectionClosedInvoked)
        {
            return;
        }

        connectionClosedInvoked = true;
        ConnectionClosed?.Invoke();
    }
}
