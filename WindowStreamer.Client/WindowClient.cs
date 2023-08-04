using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Serilog;
using WindowStreamer.Client.Exceptions;
using WindowStreamer.Image;
using WindowStreamer.Protocol;

namespace WindowStreamer.Client;

/// <summary>
/// Contains all network logic related to communicating to the server.
/// TODO: Divide into multiple classes, to make this have a single concern instead of all network logic.
/// </summary>
public class WindowClient : IDisposable
{
    static readonly int PacketCount = 128;

    readonly IPEndPoint serverEndpoint;
    readonly int framerateCapHz;
    readonly CancellationTokenSource metastreamToken;
    readonly CancellationTokenSource videostreamToken;
    readonly IImageFactory imageFactory;

    IDisposable? videoClientDisposable;
    IDisposable? tcpClientDisposable;
    bool connectionClosedCalled;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowClient"/> class.
    /// </summary>
    /// <param name="serverIP">The IP address of the server to connect to.</param>
    /// <param name="serverPort">The port of the TCP server for control messages (metastream).</param>
    /// <param name="framerateCapHz">The amount of image frames per second to ask the server to send.</param>
    /// <param name="imageFactory">Produces images from byte.</param>
    public WindowClient(IPAddress serverIP, int serverPort, int framerateCapHz, IImageFactory imageFactory)
    {
        serverEndpoint = new IPEndPoint(serverIP, serverPort);
        this.framerateCapHz = framerateCapHz;
        this.imageFactory = imageFactory;

        metastreamToken = new CancellationTokenSource();
        videostreamToken = new CancellationTokenSource();
    }

    /// <summary>
    /// Event called when a complete frame has been received.
    /// </summary>
    public event Action<IImage>? NewFrame;

    /// <summary>
    /// Event called when the resolution is changed.
    /// </summary>
    public event Action<Size>? ResolutionChanged;

    /// <summary>
    /// Event called when the client has recieved a response from server, either by message (deny/accept), or action (socket forcefully closed).
    /// </summary>
    public event Action<bool>? ConnectionAttemptFinished;

    /// <summary>
    /// Event called when the client has been disconnected from server, either by server closing, or by being kicked from server.
    /// </summary>
    public event Action? ConnectionClosed;

    /// <summary>
    /// Tries to connect to server, returns whether the connection was successfully established.
    /// <c>ConnectionAttemptFinished</c> is not invoked during this method.
    /// </summary>
    /// <returns>Whether the connection was sucessful.</returns>
    public async Task<bool> ConnectToServerAsync()
    {
        if (tcpClientDisposable is not null || videoClientDisposable is not null)
        {
            // Fatal because this should ever happen.
            Log.Fatal("Client already connected, aborting new connection");
            throw new InstanceAlreadyInUseException("This client has been connected previously. Please make a new instance instead.");
        }

        TcpClient newClient = new TcpClient();
        tcpClientDisposable = newClient;

        Log.Information($"Connecting to {serverEndpoint.Address}:{serverEndpoint.Port}...");

        try
        {
            await newClient.ConnectAsync(serverEndpoint.Address, serverEndpoint.Port);
        }
        catch (SocketException)
        {
            Log.Information("Connection unsuccessful.");
            return false;
        }

        Log.Information($"Awaiting response from {serverEndpoint}");

        Task.Run(() => MetastreamLoop(newClient), metastreamToken.Token);
        return true;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Invoke cancellation of tokens.
        metastreamToken?.Cancel();
        videostreamToken?.Cancel();

        // Dispose of network objects.
        videoClientDisposable?.Dispose();
        tcpClientDisposable?.Dispose();

        // Dispose of token sources.
        metastreamToken?.Dispose();
        videostreamToken?.Dispose();
    }

    void InvokeNewFrame(byte[] imageData, ushort width, ushort height)
    {
        Log.Debug($"New frame size: {imageData.Length}");
        IImage image = imageFactory.CreateImage(imageData, new Size(width, height));

        NewFrame?.Invoke(image);
    }

    void ClientDisconnected()
    {
        if (connectionClosedCalled)
        {
            return;
        }

        connectionClosedCalled = true;
        ConnectionClosed?.Invoke();
    }

    /// <summary>
    /// Listens for videodatagrams, is stopped when <c>videostreamToken</c> is cancelled, or <c>metaClient</c> is disconnected.
    /// Returns void because it's a loop.
    /// </summary>
    async void ListenVideoDatagramAsync(TcpClient metaClient, int videoPort)
    {
        byte[]? image = null;
        ushort imageWidth = 0;
        ushort imageHeight = 0;
        var chunks = new HashSet<ushort>();

        UdpClient videoClient = new UdpClient(videoPort);
        videoClient.Client.ReceiveBufferSize = 1_024_000;
        videoClientDisposable = videoClient;

        while (metaClient.Connected)
        {
            UdpReceiveResult received;
            try
            {
                received = await videoClient.ReceiveAsync(videostreamToken.Token);
            }
            catch (SocketException)
            {
                ClientDisconnected();
                return;
            }
            catch (OperationCanceledException)
            {
                ClientDisconnected();
                return;
            }

            ushort chunkIndex = BitConverter.ToUInt16(received.Buffer, 0);
            int totalSizeBytes = BitConverter.ToInt32(received.Buffer, sizeof(ushort));
            int chunkSizeBytes = ((totalSizeBytes - 1) / PacketCount) + 1;
            ushort width = BitConverter.ToUInt16(received.Buffer, sizeof(ushort) + sizeof(int));
            ushort height = BitConverter.ToUInt16(received.Buffer, sizeof(ushort) + sizeof(int) + sizeof(ushort));

            if (image is null || width != imageWidth || height != imageHeight)
            {
                imageWidth = width;
                imageHeight = height;
                image = new byte[totalSizeBytes];
                chunks.Clear();
            }

            chunks.Add(chunkIndex);

            int chunkOffsetBytes = chunkSizeBytes * chunkIndex;
            int imageDataOffset = sizeof(ushort) + sizeof(int) + sizeof(ushort) + sizeof(ushort);

            Log.Debug($"Chunk {chunkIndex} size: {received.Buffer.Length - imageDataOffset} / {chunkSizeBytes}, offset value: {image[chunkOffsetBytes]}");
            Buffer.BlockCopy(
                received.Buffer,
                imageDataOffset,
                image,
                chunkOffsetBytes,
                received.Buffer.Length - imageDataOffset);

            if (chunks.Count == PacketCount)
            {
                chunks.Clear();

                // Bring new frame to display on different thread to speed up packet processing on this thread.
                Task.Run(() => InvokeNewFrame((byte[])image.Clone(), imageWidth, imageHeight), videostreamToken.Token);
            }
        }
    }

    /// <summary>
    /// Listens for events from the server.
    /// Returns void because it's a loop.
    /// </summary>
    async void MetastreamLoop(TcpClient metaClient)
    {
        var stream = metaClient.GetStream();

        while (metaClient.Connected)
        {
            ServerMessage msg;
            try
            {
                msg = ServerMessage.Parser.ParseDelimitedFrom(stream);
            }
            catch (IOException ex)
            {
                Log.Debug(ex.ToString());
                ClientDisconnected();
                return;
            }
            catch (TaskCanceledException)
            {
                ClientDisconnected();
                return;
            }

            switch (msg.MsgCase)
            {
                case ServerMessage.MsgOneofCase.None:
                    Log.Debug("Received unknown message");
                    break;
                case ServerMessage.MsgOneofCase.ConnectionReply:
                    Log.Debug("Recieved connection reply");
                    var connReply = msg.ConnectionReply;

                    if (!connReply.Accepted)
                    {
                        // TODO: implement some connection ended logic.
                        Log.Information("Connection request denied :(");
                        ConnectionAttemptFinished?.Invoke(false);
                        return;
                    }

                    // Connection accepted, initialize udp-client and image-loop
                    Log.Information("Connection request accepted, awaiting handshake finish...");
                    Task.Run(() => ListenVideoDatagramAsync(metaClient, connReply.VideoPort), videostreamToken.Token);

                    // Finish handshake
                    new ClientMessage
                    {
                        UDPReady = new UDPReady
                        {
                            FramerateCap = framerateCapHz,
                        },
                    }.WriteDelimitedTo(stream);

                    Log.Information("Handshake finished");
                    ConnectionAttemptFinished?.Invoke(true);

                    break;
                case ServerMessage.MsgOneofCase.ResolutionChange:
                    Log.Debug("Recieved resolution update");

                    ResolutionChanged?.Invoke(new Size(msg.ResolutionChange.Width, msg.ResolutionChange.Height));
                    break;
            }
        }

        Log.Information("Connection lost... or disconnected");
    }
}
