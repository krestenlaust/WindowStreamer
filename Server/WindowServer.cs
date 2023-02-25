using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Protocol;
using Serilog;
using Shared;

namespace Server
{
    public class WindowServer
    {
        readonly Func<(Bitmap, Size)> obtainImage;
        readonly IPAddress boundIP;
        readonly Func<IPAddress, ConnectionReply> handleConnectionRequest;

        // TODO: Why is internetwork specified here?
        TcpClient metaClient = new TcpClient(AddressFamily.InterNetwork);
        NetworkStream metaStream;
        TcpListener clientListener;
        IPEndPoint clientEndpoint;
        UdpClient videoClient;
        bool streamVideo = true;
        Size resolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowServer"/> class.
        /// </summary>
        /// <param name="boundIP">IP address to listen on, usually <c>IPAddress.Any</c>.</param>
        /// <param name="startingResolution">The resolution of the window.</param>
        /// <param name="obtainImage">Handler for retrieving screenshots.</param>
        /// <param name="handleConnectionRequest">Handler for replying to connection attempts.</param>
        /// <param name="logger">Logging method.</param>
        public WindowServer(IPAddress boundIP, Size startingResolution, Func<(Bitmap, Size)> obtainImage, Func<IPAddress, ConnectionReply> handleConnectionRequest)
        {
            this.boundIP = boundIP;
            this.obtainImage = obtainImage;
            this.handleConnectionRequest = handleConnectionRequest;
            resolution = startingResolution;
        }

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

        public async Task StartServerAsync()
        {
            clientListener?.Stop();
            clientListener = new TcpListener(boundIP, DefaultValues.MetaStreamPort);
            clientListener.Start();
            Log.Information($"Server started {boundIP}:{DefaultValues.MetaStreamPort}");

            metaClient = await clientListener.AcceptTcpClientAsync();
            metaStream = metaClient.GetStream();
            Log.Information("Connection recieved...");

            Task.Run(MetastreamLoop);

            await HandshakeAsync();
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
            metaStream.Write(resChange, 0, resChange.Length);
        }

        const int packetCount = 32;

        void SendPicture(UdpClient client)
        {
            if (!client.Client.Connected || !streamVideo)
            {
                return;
            }

            (Bitmap bmp, Size resolution) = obtainImage();
            byte[] bytes = new byte[bmp.Height * bmp.Width * 3];

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color color = bmp.GetPixel(x, y);
                    bytes[(x + (y * bmp.Width)) * 3] = color.R;
                    bytes[((x + (y * bmp.Width)) * 3) + 1] = color.G;
                    bytes[((x + (y * bmp.Width)) * 3) + 2] = color.B;
                }
            }

            int totalSizePixels = bmp.Height * bmp.Width;
            int chunkSizePixels = ((totalSizePixels - 1) / packetCount) + 1;

            for (int i = 0; i < packetCount; i++)
            {
                int currentChunkSize = chunkSizePixels * 3;
                int chunkOffset = chunkSizePixels * 3 * i;

                // Last packet usually has different size.
                if (i == packetCount - 1)
                {
                    currentChunkSize = bytes.Length - chunkOffset;
                }

                int parameterOffset = sizeof(ushort) + sizeof(int) + sizeof(ushort) + sizeof(ushort);
                var chunk = new byte[parameterOffset + currentChunkSize];
                Buffer.BlockCopy(bytes, chunkOffset, chunk, parameterOffset, currentChunkSize);

                BitConverter.GetBytes((ushort)i).CopyTo(chunk, 0);
                BitConverter.GetBytes((int)bytes.Length).CopyTo(chunk, sizeof(ushort));
                BitConverter.GetBytes((ushort)bmp.Width).CopyTo(chunk, sizeof(ushort) + sizeof(int));
                BitConverter.GetBytes((ushort)bmp.Height).CopyTo(chunk, sizeof(ushort) + sizeof(int) + sizeof(ushort));

                client.Send(chunk, chunk.Length);
            }

            /*
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                bytes = stream.ToArray();
                Log.Information($"Image size: {bytes.Length}");
            }

            int chunkSize = ((bytes.Length - 1) / packetCount) + 1;

            for (int i = 0; i < packetCount; i++)
            {
                int currentChunkSize = chunkSize;

                // Last packet usually has different size.
                if (i == packetCount - 1)
                {
                    currentChunkSize = bytes.Length - (chunkSize * i);
                }

                // TODO: var nået her til, skulle til at finde ud af en måde at få alle bytes med.
                var chunk = new byte[sizeof(ushort) + sizeof(int) + currentChunkSize];
                Buffer.BlockCopy(bytes, chunkSize * i, chunk, sizeof(ushort) + sizeof(int), currentChunkSize);

                BitConverter.GetBytes((ushort)i).CopyTo(chunk, 0);
                BitConverter.GetBytes((int)bytes.Length).CopyTo(chunk, sizeof(ushort));

                client.Send(chunk, chunk.Length);
            }*/

            Thread.Sleep(10);
        }

        async void BeginStreamLoop()
        {
            while (streamVideo)
            {
                SendPicture(videoClient);

                await Task.Delay(50);
            }
        }

        async Task MetastreamLoop()
        {
            while (metaClient.Connected)
            {
                byte[] buffer = new byte[Constants.MetaFrameLength];
                await metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

                string[] metapacket = Encoding.UTF8.GetString(buffer).TrimEnd('\0').Split(Constants.ParameterSeparator);

                switch ((ClientPacketHeader)int.Parse(metapacket[0]))
                {
                    case ClientPacketHeader.Key:
                        Log.Debug($"Recieved key: {metapacket[1]}");
                        break;
                    case ClientPacketHeader.UDPReady:
                        Log.Debug("Udp ready!");
                        ConnectVideoStream();
                        BeginStreamLoop();
                        break;
                    default:
                        Log.Debug($"Recived this: {metapacket[0]}");
                        break;
                }
            }

            Log.Information("Connection lost... or disconnected");
        }

        /// <summary>
        /// Make handshake and begin listening loop.
        /// </summary>
        /// <returns></returns>
        async Task HandshakeAsync()
        {
            clientEndpoint = metaClient.Client.RemoteEndPoint as IPEndPoint;

            Log.Information("Inbound connection, awaiting action...");
            switch (handleConnectionRequest(clientEndpoint.Address))
            {
                case ConnectionReply.Close:

                    metaClient.Close();

                    await StartServerAsync();
                    return;

                case ConnectionReply.Accept:
                    Log.Information("Accepting connection");

                    videoClient = new UdpClient();
                    streamVideo = true;

                    var replyAccept = Send.ConnectionReply(true, resolution, DefaultValues.VideoStreamPort);
                    metaStream.Write(replyAccept, 0, replyAccept.Length);
                    break;

                case ConnectionReply.Deny:
                    var replyDeny = Send.ConnectionReply(false, Size.Empty, 0);
                    metaStream.Write(replyDeny);

                    Log.Information($"Told {clientEndpoint.Address} to try again another day :)");
                    metaClient.Close();

                    await StartServerAsync();
                    return;
                default:
                    return;
            }
        }

        void ConnectVideoStream()
        {
            videoClient.Connect(clientEndpoint.Address, DefaultValues.VideoStreamPort);
            videoClient.DontFragment = false;
        }
    }
}
