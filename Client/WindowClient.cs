using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Serilog;
using Shared;

namespace Client
{
    public class WindowClient : IDisposable
    {
        static readonly int packetCount = 8;

        readonly int metastreamPort;
        readonly IPAddress serverIP;

        int? videostreamPort;
        UdpClient videoClient;
        TcpClient metaClient;
        NetworkStream metaStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowClient"/> class.
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="metastreamPort"></param>
        /// <param name="logger"></param>
        public WindowClient(IPAddress serverIP, int metastreamPort)
        {
            this.serverIP = serverIP;
            this.metastreamPort = metastreamPort;
        }

        /// <summary>
        /// Event called when a complete frame has been received.
        /// </summary>
        public event Action<Bitmap> NewFrame;

        /// <summary>
        /// Event called when the resolution is changed.
        /// </summary>
        public event Action<Size> ResolutionChanged;

        public async Task ConnectToServerAsync()
        {
            if (metaClient?.Connected == true)
            {
                Log.Information($"Disconnecting {serverIP}:{metastreamPort}");
                metaClient.Close();
            }

            metaClient = new TcpClient();

            Log.Information($"Connecting to {serverIP}:{metastreamPort}...");

            try
            {
                await metaClient.ConnectAsync(serverIP, metastreamPort);
            }
            catch (SocketException)
            {
                Log.Information("Connection unsuccessful.");
                return;
            }

            metaStream = metaClient.GetStream();

            Log.Information($"Awaiting response from {serverIP}");

            await Task.Run(MetastreamLoop);
        }

        public void Dispose()
        {
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

        async Task ListenVideoDatagramAsync()
        {
            byte[] image = null;
            ushort imageWidth = 0;
            ushort imageHeight = 0;
            HashSet<ushort> chunks = new HashSet<ushort>();

            while (metaClient.Connected)
            {
                var received = await videoClient.ReceiveAsync();
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

                    Task.Run(() => InvokeNewFrame((byte[])image.Clone(), imageWidth, imageHeight));
                }
            }
        }

        async Task MetastreamLoop()
        {
            bool handshakeFinished = false;

            while (metaClient.Connected)
            {
                var packet = new byte[Constants.MetaFrameLength];
                // TODO: #3 - Sometimes makes the client crash when closing the server.
                await metaStream.ReadAsync(packet, 0, Constants.MetaFrameLength);

                string[] metapacket = Encoding.UTF8.GetString(packet).TrimEnd('\0').Split(Constants.ParameterSeparator);
                var packetType = (ServerPacketHeader)int.Parse(metapacket[0]);

                switch (packetType)
                {
                    case ServerPacketHeader.ConnectionReply:
                        Log.Debug($"Handshake recieved");

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
                            return;
                        }

                        Log.Information("Connection request accepted, awaiting handshake finish...");
                        IPEndPoint ipEndPoint = (IPEndPoint)metaClient.Client.RemoteEndPoint!;

                        videoClient = new UdpClient(videoPort);
                        videoClient.Client.ReceiveBufferSize = 1024_000;
                        Task.Run(ListenVideoDatagramAsync);

                        byte[] udpReady = Send.UDPReady(DefaultValues.FramerateCap);
                        metaStream.Write(udpReady, 0, udpReady.Length);

                        handshakeFinished = true;

                        Log.Information("Stream established");

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
