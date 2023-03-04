using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
        const int packetCount = 8;

        readonly Func<Bitmap> obtainImage;
        readonly IPAddress boundIP;
        readonly Func<IPAddress, ConnectionReply> handleConnectionRequest;
        readonly int metaPort;
        readonly int frameIntervalMS = 50;
        CancellationTokenSource videoStreamToken;

        // TODO: Why is internetwork specified here?
        TcpClient metaClient = new TcpClient(AddressFamily.InterNetwork);
        NetworkStream metaStream;
        TcpListener clientListener;
        IPEndPoint clientEndpoint;
        UdpClient videoClient;
        Size resolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowServer"/> class.
        /// </summary>
        /// <param name="boundIP">IP address to listen on, usually <c>IPAddress.Any</c>.</param>
        /// <param name="startingResolution">The resolution of the window.</param>
        /// <param name="obtainImage">Handler for retrieving screenshots.</param>
        /// <param name="handleConnectionRequest">Handler for replying to connection attempts.</param>
        /// <param name="logger">Logging method.</param>
        public WindowServer(IPAddress boundIP, int port, Size startingResolution, Func<Bitmap> obtainImage, Func<IPAddress, ConnectionReply> handleConnectionRequest)
        {
            this.boundIP = boundIP;
            this.obtainImage = obtainImage;
            this.handleConnectionRequest = handleConnectionRequest;
            resolution = startingResolution;
            metaPort = port;
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
            clientListener = new TcpListener(boundIP, metaPort);
            clientListener.Start();
            Log.Information($"Server started {boundIP}:{metaPort}");

            metaClient = await clientListener.AcceptTcpClientAsync();
            metaStream = metaClient.GetStream();
            Log.Information("Connection recieved...");

            await HandshakeAsync();
            await MetastreamLoop();
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

        void SendPicture(UdpClient client)
        {
            Bitmap bmp = obtainImage();
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
            }

            int totalSizePixels = bmp.Height * bmp.Width;
            int chunkSizeBytes = (((totalSizePixels * 3) - 1) / packetCount) + 1;

            for (int i = 0; i < packetCount; i++)
            {
                int chunkOffsetBytes = chunkSizeBytes * i;

                // Last packet usually has different size.
                if (i == packetCount - 1)
                {
                    chunkSizeBytes = imageDataBytes.Length - chunkOffsetBytes;
                }

                int parameterOffset = sizeof(ushort) + sizeof(int) + sizeof(ushort) + sizeof(ushort);
                var chunk = new byte[parameterOffset + chunkSizeBytes];
                Buffer.BlockCopy(imageDataBytes, chunkOffsetBytes, chunk, parameterOffset, chunkSizeBytes);

                BitConverter.GetBytes((ushort)i).CopyTo(chunk, 0);
                BitConverter.GetBytes((int)imageDataBytes.Length).CopyTo(chunk, sizeof(ushort));
                BitConverter.GetBytes((ushort)bmp.Width).CopyTo(chunk, sizeof(ushort) + sizeof(int));
                BitConverter.GetBytes((ushort)bmp.Height).CopyTo(chunk, sizeof(ushort) + sizeof(int) + sizeof(ushort));

                Log.Debug($"Chunk {i} size: {chunk.Length - parameterOffset} / {chunkSizeBytes}");

                client.Send(chunk, chunk.Length);
            }

            /*
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                imageDataBytes = stream.ToArray();
                Log.Information($"Image size: {imageDataBytes.Length}");
            }*/
        }

        public void DebugSendPicture()
        {
            SendPicture(videoClient);
        }

        async Task BeginStreamLoop(CancellationToken token)
        {
            Stopwatch sw = new Stopwatch();
            while (!token.IsCancellationRequested)
            {
                sw.Restart();
                SendPicture(videoClient);

                Log.Information(sw.ElapsedMilliseconds.ToString());
                await Task.Delay(Math.Max(frameIntervalMS - (int)sw.ElapsedMilliseconds, 0), token);
            }
        }

        async Task MetastreamLoop()
        {
            while (metaClient.Connected)
            {
                byte[] buffer = new byte[Constants.MetaFrameLength];

                // TODO: #3 - Sometimes makes the server crash when closing the client.
                await metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

                string[] metapacket = Encoding.UTF8.GetString(buffer).TrimEnd('\0').Split(Constants.ParameterSeparator);

                switch ((ClientPacketHeader)int.Parse(metapacket[0]))
                {
                    case ClientPacketHeader.Key:
                        Log.Debug($"Recieved key: {metapacket[1]}");
                        break;
                    case ClientPacketHeader.UDPReady:
                        Log.Debug("Udp ready!");

                        videoClient = new UdpClient();
                        videoClient.Client.SendBufferSize = 1024_000;
                        videoClient.DontFragment = false; // TODO: Properly remove this property assignment.
                        videoClient.Connect(clientEndpoint.Address, DefaultValues.VideoStreamPort);

                        videoStreamToken = new CancellationTokenSource();
                        Task.Run(() => BeginStreamLoop(videoStreamToken.Token));
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
    }
}
