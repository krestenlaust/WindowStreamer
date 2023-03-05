using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Protocol;
using Serilog;
using Shared;

namespace Client
{
    public class InstanceAlreadyInUseException : Exception
    {
        public InstanceAlreadyInUseException(string? msg) : base(msg)
        {
        }
    }

    public class WindowClient : IDisposable
    {
        static readonly int packetCount = 8;

        readonly IPEndPoint serverEndpoint;

        int? videostreamPort;
        UdpClient videoClient;
        TcpClient metaClient;
        NetworkStream metaStream;

        CancellationTokenSource metastreamToken;
        CancellationTokenSource videostreamToken;
        Task taskMetastream;
        Task taskVideostream;

        bool connectionClosedCalled;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowClient"/> class.
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="metastreamPort"></param>
        /// <param name="logger"></param>
        public WindowClient(IPAddress serverIP, int metastreamPort)
        {
            serverEndpoint = new IPEndPoint(serverIP, metastreamPort);
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

            metaStream = metaClient.GetStream();

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
            metaStream?.Dispose();
        }

        void InvokeNewFrame(byte[] imageData, ushort width, ushort height)
        {
            Log.Information(imageData.Length.ToString());
            var bitmap = new Bitmap(width, height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    int i = (y * bitmap.Width * 3) + x * 3;

                    bitmap.SetPixel(x, y, Color.FromArgb(imageData[i], imageData[i + 1], imageData[i + 2]));
                }
            }

            NewFrame?.Invoke(bitmap);
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
            HashSet<ushort> chunks = new HashSet<ushort>();

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

                if (videostreamToken.Token.IsCancellationRequested)
                {
                    ClientDisconnected();
                    return;
                }

                ushort chunkIndex = BitConverter.ToUInt16(received.Buffer, 0);
                int totalSizeBytes = BitConverter.ToInt32(received.Buffer, sizeof(ushort));
                int chunkSizeBytes = ((totalSizeBytes - 1) / packetCount) + 1;
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

                if (chunks.Count == packetCount)
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
                var packet = new byte[Constants.MetaFrameLength];

                try
                {
                    await metaStream.ReadAsync(packet, 0, Constants.MetaFrameLength, metastreamToken.Token);
                }
                catch (IOException)
                {
                    ClientDisconnected();
                    return;
                }

                if (metastreamToken.Token.IsCancellationRequested)
                {
                    ClientDisconnected();
                    return;
                }

                string[] metapacket = Encoding.UTF8.GetString(packet).TrimEnd('\0').Split(Constants.ParameterSeparator);
                var packetType = (ServerPacketHeader)int.Parse(metapacket[0]);

                switch (packetType)
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

                        byte[] udpReady = Send.UDPReady(DefaultValues.FramerateCap);
                        metaStream.Write(udpReady, 0, udpReady.Length);

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
}
