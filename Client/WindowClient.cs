﻿using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Serilog;
using Shared;

namespace Client
{
    public class WindowClient : IDisposable
    {
        readonly int metastreamPort;
        readonly IPAddress serverIP;

        int? videostreamPort;
        UdpClient videoClient;
        TcpClient metaClient;
        NetworkStream metaStream;
        Bitmap bitmap;

        static readonly int packetCount = 32;
        //byte[] wholePacket;
        //int chunksReceived;
        // Used for debugging
        int packetsReceived;

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
        //public event Action<byte[]> NewFrame;

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

        void RecieveDatagram(IAsyncResult res)
        {
            var endPoint = new IPEndPoint(serverIP, videostreamPort!.Value);
            byte[] received = videoClient.EndReceive(res, ref endPoint!);
            packetsReceived++;

            ushort chunkIndex = BitConverter.ToUInt16(received, 0);
            int totalSizeBytes = BitConverter.ToInt32(received, sizeof(ushort));
            int chunkSizeBytes = ((totalSizeBytes - 1) / packetCount) + 1;
            ushort width = BitConverter.ToUInt16(received, sizeof(ushort) + sizeof(int));
            ushort height = BitConverter.ToUInt16(received, sizeof(ushort) + sizeof(int) + sizeof(ushort));

            int chunkOffsetBytes = chunkSizeBytes * chunkIndex;
            int imageDataOffset = sizeof(ushort) + sizeof(int) + sizeof(ushort) + sizeof(ushort);

            Span<byte> imageData = new Span<byte>(
                received,
                imageDataOffset,
                received.Length - imageDataOffset);

            if (bitmap is null || bitmap.Width != width || bitmap.Height != height)
            {
                bitmap = new Bitmap(width, height);
            }

            for (int i = 0; i < imageData.Length; i += 3)
            {
                Color color = Color.FromArgb(imageData[i], imageData[i + 1], imageData[i + 2]);

                // The server-side should be correct, it's somewhere around here the problem lies.
                (int y, int x) = Math.DivRem((i / 3) + chunkOffsetBytes, width);

                bitmap.SetPixel(x, y, color);
            }

            NewFrame?.Invoke((Bitmap)bitmap.Clone());

            videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);
        }

        /*void RecieveDatagram(IAsyncResult res)
        {
            var endPoint = new IPEndPoint(serverIP, videostreamPort!.Value);
            byte[] received = videoClient.EndReceive(res, ref endPoint!);
            packetsReceived++;

            ushort chunkIndex = BitConverter.ToUInt16(received, 0);
            int totalSizeBytes = BitConverter.ToInt32(received, sizeof(ushort));
            int chunkSizeBytes = ((totalSizeBytes - 1) / packetCount) + 1;

            if (wholePacket is null)
            {
                wholePacket = new byte[totalSizeBytes];
            }

            Array src = received;
            Array dst = wholePacket;
            int srcOffset = sizeof(ushort) + sizeof(int);
            int dstOffset = chunkSizeBytes * chunkIndex;
            int count = received.Length - (sizeof(ushort) + sizeof(int));

            // #5 Implementing chunking. #16 comment.
            Buffer.BlockCopy(src, srcOffset, dst, dstOffset, count);

            // Packet is assembled
            if (chunksReceived == packetCount - 1)
            {
                NewFrame?.Invoke(wholePacket);
                wholePacket = null;
                chunksReceived = -1;
            }

            chunksReceived++;

            videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);
        }*/

        async Task MetastreamLoop()
        {
            bool handshakeFinished = false;

            while (metaClient.Connected)
            {
                var packet = new byte[Constants.MetaFrameLength];
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
                        videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);

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
