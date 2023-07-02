﻿using Serilog;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using WindowStreamer.Protocol;

namespace WindowStreamer.Client;

public class InstanceAlreadyInUseException : Exception
{
    public InstanceAlreadyInUseException(string msg)
        : base(msg)
    {
    }
}

public class WindowClient : IDisposable
{
    static readonly int DefaultMetastreamPort = 10063;
    static readonly int PacketCount = 128;

    readonly IPEndPoint serverEndpoint;
    readonly int framerateCap;

    UdpClient videoClient;
    TcpClient metaClient;
    int? videostreamPort;

    CancellationTokenSource metastreamToken;
    CancellationTokenSource videostreamToken;
    Task taskMetastream;
    Task taskVideostream;
    bool connectionClosedCalled;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowClient"/> class.
    /// </summary>
    /// <param name="serverIP">The IP address of the server to connect to.</param>
    /// <param name="serverPort">The port of the TCP server for control messages (metastream).</param>
    /// <param name="framerateCap">The amount of image frames per second to ask the server to send.</param>
    public WindowClient(IPAddress serverIP, int serverPort, int framerateCap)
    {
        serverEndpoint = new IPEndPoint(serverIP, serverPort);
        this.framerateCap = framerateCap;
    }

    /// <summary>
    /// Event called when a complete frame has been received.
    /// </summary>
    public event Action<Bitmap> NewFrame;

    /// <summary>
    /// Event called when the resolution is changed.
    /// </summary>
    public event Action<Size> ResolutionChanged;

    /// <summary>
    /// Event called when the client has recieved a response from server, either by message (deny/accept), or action (socket forcefully closed).
    /// </summary>
    public event Action<bool> ConnectionAttemptFinished;

    /// <summary>
    /// Event called when the client has been disconnected from server, either by server closing, or by being kicked from server.
    /// </summary>
    public event Action ConnectionClosed;

    /// <summary>
    /// Tries to connect to server, returns whether the connection was successfully established.
    /// <c>ConnectionAttemptFinished</c> is not invoked during this method.
    /// </summary>
    /// <returns>Whether the connection was sucessful.</returns>
    public async Task<bool> ConnectToServerAsync()
    {
        if (metaClient is not null || videoClient is not null)
        {
            // Fatal because this should ever happen.
            Log.Fatal("Client already connected, aborting new connection");
            throw new InstanceAlreadyInUseException("This client has been connected previously. Please make a new instance instead.");
        }

        metaClient = new TcpClient();

        Log.Information($"Connecting to {serverEndpoint.Address}:{serverEndpoint.Port}...");

        try
        {
            await metaClient.ConnectAsync(serverEndpoint.Address, serverEndpoint.Port);
        }
        catch (SocketException)
        {
            Log.Information("Connection unsuccessful.");
            return false;
        }

        Log.Information($"Awaiting response from {serverEndpoint}");

        metastreamToken = new CancellationTokenSource();
        taskMetastream = Task.Run(MetastreamLoop, metastreamToken.Token);
        return true;
    }

    public void Dispose()
    {
        metastreamToken?.Cancel();
        videostreamToken?.Cancel();

        videoClient?.Dispose();
        metaClient?.Dispose();
    }

    void InvokeNewFrame(byte[] imageData, ushort width, ushort height)
    {
        Log.Debug($"New frame size: {imageData.Length}");
        var bmp = new Bitmap(width, height);

        BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

        IntPtr imageDataPtr = bmpData.Scan0;

        Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);
        bmp.UnlockBits(bmpData);

        /*
        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                int i = (y * bmp.Width * 3) + x * 3;

                bmp.SetPixel(x, y, Color.FromArgb(imageData[i], imageData[i + 1], imageData[i + 2]));
            }
        }*/

        NewFrame?.Invoke(bmp);
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
    async void ListenVideoDatagramAsync()
    {
        byte[] image = null;
        ushort imageWidth = 0;
        ushort imageHeight = 0;
        var chunks = new HashSet<ushort>();

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
    async void MetastreamLoop()
    {
        bool handshakeFinished = false;

        while (metaClient.Connected)
        {
            var stream = metaClient.GetStream();
            var packet = new byte[Constants.MetaFrameLength];

            int readBytes;

            try
            {
                readBytes = await stream.ReadAsync(packet, 0, Constants.MetaFrameLength, metastreamToken.Token);
            }
            catch (IOException)
            {
                ClientDisconnected();
                return;
            }
            catch (TaskCanceledException)
            {
                ClientDisconnected();
                return;
            }

            // As far as I know, when 0 bytes has been read and the reading function isn't blocking, the connection has stopped.
            if (readBytes == 0)
            {
                ClientDisconnected();
                return;
            }

            string[] metapacket = Encoding.UTF8.GetString(packet).TrimEnd('\0').Split(Constants.ParameterSeparator);

            if (!int.TryParse(metapacket[0], out int packetTypeValue))
            {
                Log.Debug($"Received invalid packet[0]: {metapacket[0]}");
                continue;
            }

            switch ((ServerPacketHeader)packetTypeValue)
            {
                case ServerPacketHeader.ConnectionReply:
                    if (Parse.TryParseConnectionReply(metapacket, out bool accepted, out Size resolution, out int videoPort))
                    {
                        videostreamPort = videoPort;
                        ResolutionChanged?.Invoke(resolution);
                    }
                    else
                    {
                        Log.Debug("Failed to parse packet");
                        continue;
                    }

                    if (!accepted)
                    {
                        // TODO: implement some connection ended logic.
                        Log.Information("Connection request denied :(");
                        ConnectionAttemptFinished?.Invoke(false);
                        return;
                    }

                    Log.Information("Connection request accepted, awaiting handshake finish...");

                    // Initialize client and loop
                    videoClient = new UdpClient(videoPort);
                    videoClient.Client.ReceiveBufferSize = 1_024_000;
                    videostreamToken = new CancellationTokenSource();
                    taskVideostream = Task.Run(ListenVideoDatagramAsync, videostreamToken.Token);

                    byte[] udpReady = Send.UDPReady(framerateCap);
                    stream.Write(udpReady, 0, udpReady.Length);

                    handshakeFinished = true;
                    Log.Information("Stream established");

                    ConnectionAttemptFinished?.Invoke(true);
                    break;
                case ServerPacketHeader.ResolutionUpdate when handshakeFinished:
                    Log.Debug("Recieved resolution update");

                    if (Parse.TryParseResolutionChange(metapacket, out Size newResolution))
                    {
                        ResolutionChanged?.Invoke(newResolution);
                    }
                    else
                    {
                        Log.Debug("Failed to parse packet");
                    }

                    break;
                default:
                    Log.Debug($"Recieved this: {metapacket[0]}");
                    break;
            }
        }

        Log.Information("Connection lost... or disconnected");
    }
}