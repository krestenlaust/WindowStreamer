﻿using System.Diagnostics;
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
    public static readonly int DefaultVideoStreamPort = 10064;

    readonly IScreenshotQuery screenshotQuery;
    readonly IPEndPoint boundEndpoint;
    readonly IConnectionHandler connectionHandler;
    readonly Size startingResolution;
    readonly UdpClient udpClient;
    Task? videostreamTask;
    CancellationTokenSource videostreamToken;
    int frameIntervalMS = 34;

    TcpListener? clientListener;
    bool objectDisposed;

    ConnectedClient? connectedClient;
    CancellationTokenSource? listeningToken;

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

        udpClient = new UdpClient();
        udpClient.Client.SendBufferSize = 1024_000;
        udpClient.DontFragment = false; // TODO: Properly remove this property assignment.
    }

    /// <summary>
    /// Called whenever an active connection has been closed.
    /// Is only called for connections that completed a handshake.
    /// </summary>
    public event Action? ConnectionClosed;

    /// <summary>
    /// Gets a value indicating whether a connection has been initiated.
    /// </summary>
    [Obsolete("Not implemneted yet")]
    public bool Connected { get; private set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (objectDisposed)
        {
            return;
        }

        objectDisposed = true;

        connectedClient?.Dispose();
        udpClient?.Dispose();

        // Halt server
        videostreamToken?.Cancel();
        listeningToken?.Cancel();

        // Dispose of networked instances
        clientListener?.Stop();

        // Dispose of token sources
        videostreamToken?.Dispose();
        listeningToken?.Dispose();
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

        videostreamToken = new CancellationTokenSource();
        videostreamTask = Task.Run(BeginStreamLoop, videostreamToken.Token);

        listeningToken = new CancellationTokenSource();
        clientListener = new TcpListener(boundEndpoint);
        clientListener.Start();
        Log.Information($"Server started {boundEndpoint}");

        try
        {
            ConnectionReply reply;
            TcpClient? client;

            do
            {
                client = await clientListener.AcceptTcpClientAsync(listeningToken.Token);
                IPEndPoint endpoint = (IPEndPoint)client.Client.RemoteEndPoint!;

                Log.Information($"Connection recieved: {endpoint}, awaiting action");
                reply = connectionHandler.HandleIncomingConnection(endpoint.Address);
            }
            while (!HandshakeAsync(reply, client));

            Log.Information("Stream established");

            connectedClient = new ConnectedClient(client);
            connectedClient.ConnectionClosed += ConnectionClosed;
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
        connectedClient?.UpdateResolution(resolution);
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

    void SendPicture(byte[] rawImageData24bpp, int width, int height, IPEndPoint targetEndpoint)
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

            udpClient.Send(chunk, chunk.Length, targetEndpoint);
        }
    }

    async void BeginStreamLoop()
    {
        CancellationToken token = videostreamToken.Token;
        var sw = new Stopwatch();

        while (!token.IsCancellationRequested)
        {
            // Is client connected and ready to receive?
            if (connectedClient is null || !connectedClient.UdpReady)
            {
                try
                {
                    await Task.Delay(0);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                continue;
            }

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
                SendPicture(rawImageData, bmp.Width, bmp.Height, connectedClient.UDPEndPoint);
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

    /// <summary>
    /// Complete handshake based given replyAction-action.
    /// </summary>
    /// <returns>Whether the handshake resulted in a connection.</returns>
    bool HandshakeAsync(ConnectionReply replyAction, TcpClient networkClient)
    {
        var stream = networkClient.GetStream();
        IPEndPoint endPoint = (IPEndPoint)networkClient.Client.RemoteEndPoint!;

        switch (replyAction)
        {
            case ConnectionReply.Close:
                Log.Debug($"Closed connection from {endPoint.Address}");
                networkClient.Close();

                return false;
            case ConnectionReply.Accept:
                Log.Debug($"Accepted connection from {endPoint.Address}");

                // Accept connection
                new ServerMessage
                {
                    ConnectionReply = new Protocol.ConnectionReply
                    {
                        Accepted = true,
                        VideoPort = DefaultVideoStreamPort, // TODO: Make the client decide the video port.
                    },
                }.WriteDelimitedTo(stream);

                // Send latest resolution
                new ServerMessage
                {
                    ResolutionChange = new ResolutionChange
                    {
                        Width = startingResolution.Width,
                        Height = startingResolution.Height,
                    },
                }.WriteDelimitedTo(stream);

                return true;
            case ConnectionReply.Deny:
                Log.Debug($"Denied connection from {endPoint.Address}");

                // Deny connection attempt
                new ServerMessage
                {
                    ConnectionReply = new Protocol.ConnectionReply
                    {
                        Accepted = false,
                    },
                }.WriteDelimitedTo(stream);

                networkClient.Close();
                return false;
            default:
                return false;
        }
    }
}
