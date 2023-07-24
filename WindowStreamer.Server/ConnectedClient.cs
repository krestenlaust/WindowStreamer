using System.Drawing;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Serilog;
using WindowStreamer.Protocol;

namespace WindowStreamer.Server;

internal class ConnectedClient : IDisposable
{
    CancellationTokenSource metastreamToken;
    Task? metastreamTask;
    bool connectionClosedInvoked;
    bool objectDisposed;

    public ConnectedClient(TcpClient client)
    {
        this.NetworkClient = client;
        EndPoint = (IPEndPoint)client.Client.LocalEndPoint!;

        metastreamToken = new CancellationTokenSource();
        metastreamTask = Task.Run(() => MetastreamLoop(client.GetStream()), metastreamToken.Token);
    }

    /// <summary>
    /// Called whenever an active connection has been closed.
    /// Is only called for connections that completed a handshake.
    /// </summary>
    public event Action? ConnectionClosed;

    public IPEndPoint EndPoint { get; }

    public IPEndPoint UDPEndPoint => new IPEndPoint(EndPoint.Address, WindowServer.DefaultVideoStreamPort);

    public TcpClient NetworkClient { get; }

    public bool UdpReady { get; private set; }

    public int FramerateCapHertz { get; private set; }

    public void Dispose()
    {
        if (objectDisposed)
        {
            return;
        }

        objectDisposed = true;

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
    public void UpdateResolution(Size resolution)
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

                    FramerateCapHertz = udpReady.FramerateCap;
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
