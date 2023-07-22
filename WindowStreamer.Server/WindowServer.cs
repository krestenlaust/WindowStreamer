using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Google.Protobuf;
using Serilog;
using WindowStreamer.Protocol;
using WindowStreamer.Server.Exceptions;

namespace WindowStreamer.Server;

public class WindowServer : IDisposable
{
    const int PacketCount = 128;
    static readonly int DefaultVideoStreamPort = 10064;

    readonly IScreenshotQuery screenshotQuery;
    readonly IPEndPoint boundEndpoint;
    readonly IConnectionHandler connectionHandler;
    readonly Size startingResolution;
    int frameIntervalMS = 34;

    IPEndPoint clientEndpoint;
    TcpListener clientListener;
    TcpClient metaClient;
    UdpClient videoClient;
    bool connectionClosedInvoked;
    bool objectDisposed;

    Task metastreamTask;
    Task videostreamTask;
    CancellationTokenSource videostreamToken;
    CancellationTokenSource metastreamToken;
    CancellationTokenSource listeningToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowServer"/> class.
    /// </summary>
    /// <param name="boundIP">IP address to listen on, usually <c>IPAddress.Any</c>.</param>
    /// <param name="port">Port to bind.</param>
    /// <param name="startingResolution">The resolution of the window.</param>
    /// <param name="screenshotQuery">Handler for retrieving screenshots.</param>
    /// <param name="connectionHandler">Handler for replying to connection attempts.</param>
    public WindowServer(IPAddress boundIP, int port, Size startingResolution, IScreenshotQuery screenshotQuery, IConnectionHandler connectionHandler)
    {
        boundEndpoint = new IPEndPoint(boundIP, port);
        this.screenshotQuery = screenshotQuery;
        this.connectionHandler = connectionHandler;
        this.startingResolution = startingResolution;
    }

    /// <summary>
    /// Called whenever an active connection has been closed.
    /// Is only called for connections that completed a handshake.
    /// </summary>
    public event Action ConnectionClosed;

    /// <summary>
    /// Gets a value indicating whether a connection has been initiated.
    /// </summary>
    [Obsolete("Not implemneted yet")]
    public bool Connected { get; private set; }

    public void Dispose()
    {
        if (objectDisposed)
        {
            return;
        }

        objectDisposed = true;

        // Halt server
        listeningToken?.Cancel();
        metastreamToken?.Cancel();
        videostreamToken?.Cancel();

        // Dispose of networked instances
        clientListener?.Stop();
        metaClient?.Dispose();
        videoClient?.Dispose();

        // Dispose of token sources
        listeningToken?.Dispose();
        metastreamToken?.Dispose();
        videostreamToken?.Dispose();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InstanceAlreadyInUseException">This method-instance has been called previously.</exception>
    /// <exception cref="SocketException">Socket already registered.</exception>
    public async Task StartServerAsync()
    {
        if (clientListener is not null)
        {
            throw new InstanceAlreadyInUseException($"{nameof(clientListener)} is not null. Can'zero reuse same instance.");
        }

        listeningToken = new CancellationTokenSource();
        clientListener = new TcpListener(boundEndpoint);
        clientListener.Start();
        Log.Information($"Server started {boundEndpoint}");

        try
        {
            ConnectionReply reply;
            do
            {
                metaClient = await clientListener.AcceptTcpClientAsync(listeningToken.Token);
                clientEndpoint = metaClient.Client.RemoteEndPoint as IPEndPoint;
                Log.Information($"Connection recieved: {clientEndpoint}, awaiting action");
                reply = connectionHandler.HandleIncomingConnection(clientEndpoint.Address);
            }
            while (!await HandshakeAsync(reply));

            Log.Information("Stream established");

            metastreamToken = new CancellationTokenSource();
            metastreamTask = Task.Run(MetastreamLoop);
        }
        catch (SocketException ex)
        {
            Log.Error($"Server start crashed with following exception: {ex}");
            Dispose();
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>
    /// Notifies client of resolution change.
    /// </summary>
    /// <param name="resolution">The new resolution.</param>
    public void UpdateResolution(Size resolution)
    {
        // Notifies client of resolution change.
        if (metaClient is null || !metaClient.Connected)
        {
            Log.Debug("Not connected...");
            return;
        }

        var stream = metaClient.GetStream();
        new ServerMessage
        {
            ResolutionChange = new ResolutionChange
            {
                Width = resolution.Width,
                Height = resolution.Height,
            },
        }.WriteDelimitedTo(stream);
    }

    static byte[] ConvertBitmapToRawPixel24bpp(Bitmap bmp)
    {
        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);

        IntPtr imageDataPtr = bmpData.Scan0;
        int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
        byte[] imageDump = new byte[byteCount];

        Marshal.Copy(imageDataPtr, imageDump, 0, byteCount);
        bmp.UnlockBits(bmpData);

        return imageDump;
    }

    static void SendPicture(byte[] rawImageData24bpp, int width, int height, UdpClient client)
    {
        int chunkSizeBytes = ((rawImageData24bpp.Length - 1) / PacketCount) + 1;

        for (int i = 0; i < PacketCount; i++)
        {
            int chunkOffsetBytes = chunkSizeBytes * i;

            // Last packet usually has different size.
            if (i == PacketCount - 1)
            {
                chunkSizeBytes = rawImageData24bpp.Length - chunkOffsetBytes;
            }

            int parameterOffset = sizeof(ushort) + sizeof(int) + sizeof(ushort) + sizeof(ushort);
            var chunk = new byte[parameterOffset + chunkSizeBytes];
            Buffer.BlockCopy(rawImageData24bpp, chunkOffsetBytes, chunk, parameterOffset, chunkSizeBytes);

            BitConverter.GetBytes((ushort)i).CopyTo(chunk, 0);
            BitConverter.GetBytes((int)rawImageData24bpp.Length).CopyTo(chunk, sizeof(ushort));
            BitConverter.GetBytes((ushort)width).CopyTo(chunk, sizeof(ushort) + sizeof(int));
            BitConverter.GetBytes((ushort)height).CopyTo(chunk, sizeof(ushort) + sizeof(int) + sizeof(ushort));

            Log.Debug($"Chunk {i} size: {chunk.Length - parameterOffset} / {chunkSizeBytes}");

            client.Send(chunk, chunk.Length);
        }
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

    async void BeginStreamLoop()
    {
        CancellationToken token = videostreamToken.Token;

        var sw = new Stopwatch();
        while (!token.IsCancellationRequested)
        {
            sw.Restart();

            var obtainImageSw = new Stopwatch();
            obtainImageSw.Start();
            Bitmap bmp = screenshotQuery.GetImage();
            obtainImageSw.Stop();

            var convertPictureSw = new Stopwatch();
            convertPictureSw.Start();
            byte[] rawImageData = ConvertBitmapToRawPixel24bpp(bmp);
            convertPictureSw.Stop();

            try
            {
                SendPicture(rawImageData, bmp.Width, bmp.Height, videoClient);
            }
            catch (ObjectDisposedException ex)
            {
                Log.Debug($"Stream looped invoked exception: {ex}");
                return;
            }

            Log.Debug($"Obtain image: {obtainImageSw.ElapsedMilliseconds} ms, convert image: {convertPictureSw.ElapsedMilliseconds} ms");

            try
            {
                await Task.Delay(Math.Max(frameIntervalMS - (int)sw.ElapsedMilliseconds, 0), token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }

    async void MetastreamLoop()
    {
        var stream = metaClient.GetStream();

        while (metaClient.Connected)
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

                    frameIntervalMS = 1000 / udpReady.FramerateCap;

                    videoClient = new UdpClient();
                    videoClient.Client.SendBufferSize = 1024_000;
                    videoClient.DontFragment = false; // TODO: Properly remove this property assignment.
                    videoClient.Connect(clientEndpoint.Address, DefaultVideoStreamPort);

                    videostreamToken = new CancellationTokenSource();
                    videostreamTask = Task.Run(BeginStreamLoop, videostreamToken.Token);
                    break;
                default:
                    break;
            }
        }

        Log.Information("Connection lost... or disconnected");
    }

    /// <summary>
    /// Complete handshake based given replyAction-action.
    /// </summary>
    /// <returns></returns>
    async ValueTask<bool> HandshakeAsync(ConnectionReply replyAction)
    {
        var stream = metaClient.GetStream();

        switch (replyAction)
        {
            case ConnectionReply.Close:
                Log.Debug($"Closed connection from {clientEndpoint.Address}");
                metaClient.Close();

                return false;
            case ConnectionReply.Accept:
                Log.Debug($"Accepted connection from {clientEndpoint.Address}");

                // Accept connection
                new ServerMessage
                {
                    ConnectionReply = new Protocol.ConnectionReply
                    {
                        Accepted = true,
                        VideoPort = DefaultVideoStreamPort,
                    },
                }.WriteDelimitedTo(stream);

                // Send latest resolution
                UpdateResolution(startingResolution);

                return true;
            case ConnectionReply.Deny:
                Log.Debug($"Denied connection from {clientEndpoint.Address}");

                // Deny connection attempt
                new ServerMessage
                {
                    ConnectionReply = new Protocol.ConnectionReply
                    {
                        Accepted = false,
                    },
                }.WriteDelimitedTo(stream);

                metaClient.Close();
                return false;
            default:
                return false;
        }
    }
}
