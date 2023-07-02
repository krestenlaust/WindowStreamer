﻿using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Serilog;
using WindowStreamer.Protocol;

namespace WindowStreamer.Server;

public class InstanceAlreadyInUseException : Exception
{
    public InstanceAlreadyInUseException(string msg)
        : base(msg)
    {
    }
}

public class WindowServer : IDisposable
{
    const int PacketCount = 128;
    static readonly int DefaultVideoStreamPort = 10064;

    readonly Func<Bitmap> obtainImage;
    readonly IPEndPoint boundEndpoint;
    readonly Func<IPAddress, ConnectionReply> handleConnectionRequest;
    int frameIntervalMS = 34;

    IPEndPoint clientEndpoint;
    TcpListener clientListener;
    TcpClient metaClient;
    UdpClient videoClient;
    Size resolution;
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
    /// <param name="obtainImage">Handler for retrieving screenshots.</param>
    /// <param name="handleConnectionRequest">Handler for replying to connection attempts.</param>
    public WindowServer(IPAddress boundIP, int port, Size startingResolution, Func<Bitmap> obtainImage, Func<IPAddress, ConnectionReply> handleConnectionRequest)
    {
        boundEndpoint = new IPEndPoint(boundIP, port);
        this.obtainImage = obtainImage;
        this.handleConnectionRequest = handleConnectionRequest;
        resolution = startingResolution;
    }

    /// <summary>
    /// Called whenever an active connection has been closed.
    /// Is only called for connections that completed a handshake.
    /// </summary>
    public event Action ConnectionClosed;

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
                reply = handleConnectionRequest(clientEndpoint.Address);
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

    public void UpdateResolution(Size resolution)
    {
        // Notifies client of resolution change.
        if (metaClient?.Connected == false)
        {
            Log.Debug("Not connected...");
            return;
        }

        var resChange = Send.ResolutionChange(resolution);
        var stream = metaClient.GetStream();
        stream.Write(resChange, 0, resChange.Length);
    }

    static byte[] ConvertBitmapToRawPixel24bpp(Bitmap bmp)
    {
        /*
        byte[] imageDataBytes = new byte[bmp.Height * bmp.Width * 3];

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color color = bmp.GetPixel(x, y);
                imageDataBytes[(x + (y * bmp.Width)) * 3] = color.R;
                imageDataBytes[((x + (y * bmp.Width)) * 3) + 1] = color.G;
                imageDataBytes[((x + (y * bmp.Width)) * 3) + 2] = color.B;
            }
        }*/

        BitmapData bmpData = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);

        IntPtr imageDataPtr = bmpData.Scan0;
        int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
        byte[] imageDump = new byte[byteCount];

        Marshal.Copy(imageDataPtr, imageDump, 0, byteCount);
        bmp.UnlockBits(bmpData);

        /*
        byte[] imageDataBytes2 = new byte[bmp.Height * bmp.Width * 3];

        int dstIndex = 0;
        for (int i = 0; i < imageDump.Length; i++)
        {
            if ((i + 1) % bmpData.Stride == 0 && i != 0)
            {
                // TODO: ultra mystisk fejl
                var zero = imageDump[i];
                //imageDataBytes2[i - 1]
            }
            else
            {
                imageDataBytes2[dstIndex] = imageDump[i];
                dstIndex++;
            }
        }*/

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
            Bitmap bmp = obtainImage();
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
        while (metaClient.Connected)
        {
            var stream = metaClient.GetStream();
            byte[] buffer = new byte[Constants.MetaFrameLength];

            try
            {
                await stream.ReadAsync(buffer, 0, Constants.MetaFrameLength, metastreamToken.Token);
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

            string[] metapacket = Encoding.UTF8.GetString(buffer).TrimEnd('\0').Split(Constants.ParameterSeparator);

            switch ((ClientPacketHeader)int.Parse(metapacket[0]))
            {
                case ClientPacketHeader.Key:
                    Log.Debug($"Recieved key: {metapacket[1]}");
                    break;
                case ClientPacketHeader.UDPReady:
                    Log.Debug("Udp ready!");

                    if (!Parse.TryParseUDPReady(metapacket, out int framerateCap))
                    {

                        Log.Information("Invalid UDPReady packet");
                        break;
                    }

                    frameIntervalMS = 1000 / framerateCap;

                    videoClient = new UdpClient();
                    videoClient.Client.SendBufferSize = 1024_000;
                    videoClient.DontFragment = false; // TODO: Properly remove this property assignment.
                    videoClient.Connect(clientEndpoint.Address, DefaultVideoStreamPort);

                    videostreamToken = new CancellationTokenSource();
                    videostreamTask = Task.Run(BeginStreamLoop, videostreamToken.Token);
                    break;
                default:
                    Log.Debug($"Received this: {metapacket[0]}");
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

                var replyAccept = Send.ConnectionReply(true, resolution, DefaultVideoStreamPort);
                await stream.WriteAsync(replyAccept);
                return true;

            case ConnectionReply.Deny:
                Log.Debug($"Denied connection from {clientEndpoint.Address}");
                var replyDeny = Send.ConnectionReply(false, Size.Empty, 0);
                await stream.WriteAsync(replyDeny);

                metaClient.Close();

                return false;

            default:
                return false;
        }
    }
}