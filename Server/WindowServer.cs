using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Protocol;
using Serilog;
using Shared;

namespace Server
{
    public class InstanceAlreadyInUseException : Exception
    {
        public InstanceAlreadyInUseException(string msg)
            : base(msg)
        {
        }
    }

    public class WindowServer : IDisposable
    {
        const int packetCount = 128;

        readonly Func<Bitmap> obtainImage;
        readonly IPEndPoint boundEndpoint;
        readonly Func<IPAddress, ConnectionReply> handleConnectionRequest;
        readonly int frameIntervalMS = 34;

        IPEndPoint clientEndpoint;
        TcpListener clientListener;
        TcpClient metaClient;
        UdpClient videoClient;
        Size resolution;
        bool connectionClosedInvoked;

        Task metastreamTask;
        Task videostreamTask;
        CancellationTokenSource videostreamToken;
        CancellationTokenSource metastreamToken;

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
            metastreamToken?.Cancel();
            videostreamToken?.Cancel();

            metaClient?.Dispose();
            videoClient?.Dispose();
        }

        public async Task StartServerAsync()
        {
            if (clientListener is not null)
            {
                throw new InstanceAlreadyInUseException($"{nameof(clientListener)} is not null. Can'zero reuse same instance.");
            }

            clientListener = new TcpListener(boundEndpoint);
            clientListener.Start();
            Log.Information($"Server started {boundEndpoint}");

            ConnectionReply reply;
            do
            {
                metaClient = await clientListener.AcceptTcpClientAsync();
                clientEndpoint = metaClient.Client.RemoteEndPoint as IPEndPoint;
                Log.Information($"Connection recieved: {clientEndpoint}");
                reply = handleConnectionRequest(clientEndpoint.Address);
            }
            while (!await HandshakeAsync(reply));

            metastreamToken = new CancellationTokenSource();
            metastreamTask = Task.Run(MetastreamLoop);
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
                DefaultValues.ImageFormat);

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
            int totalSizePixels = width * height;
            int chunkSizeBytes = ((rawImageData24bpp.Length - 1) / packetCount) + 1;

            for (int i = 0; i < packetCount; i++)
            {
                int chunkOffsetBytes = chunkSizeBytes * i;

                // Last packet usually has different size.
                if (i == packetCount - 1)
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

            Stopwatch sw = new Stopwatch();
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

                SendPicture(rawImageData, bmp.Width, bmp.Height, videoClient);

                Log.Information($"Obtain image: {obtainImageSw.ElapsedMilliseconds} ms, convert image: {convertPictureSw.ElapsedMilliseconds} ms");
                await Task.Delay(Math.Max(frameIntervalMS - (int)sw.ElapsedMilliseconds, 0), token);
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
                    ClientDisconnected();
                    return;
                }

                if (metastreamToken.Token.IsCancellationRequested)
                {
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

                        videoClient = new UdpClient();
                        videoClient.Client.SendBufferSize = 1024_000;
                        videoClient.DontFragment = false; // TODO: Properly remove this property assignment.
                        videoClient.Connect(clientEndpoint.Address, DefaultValues.VideoStreamPort);

                        videostreamToken = new CancellationTokenSource();
                        videostreamTask = Task.Run(BeginStreamLoop, videostreamToken.Token);
                        break;
                    default:
                        Log.Debug($"Recived this: {metapacket[0]}");
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

            Log.Information("Inbound connection, awaiting action...");
            switch (replyAction)
            {
                case ConnectionReply.Close:
                    metaClient.Close();

                    Log.Information("Closed connection");
                    return false;

                case ConnectionReply.Accept:
                    Log.Information("Accepting connection");

                    var replyAccept = Send.ConnectionReply(true, resolution, DefaultValues.VideoStreamPort);
                    await stream.WriteAsync(replyAccept);
                    return true;

                case ConnectionReply.Deny:
                    var replyDeny = Send.ConnectionReply(false, Size.Empty, 0);
                    await stream.WriteAsync(replyDeny);

                    Log.Information($"Told {clientEndpoint.Address} to try again another day :)");
                    metaClient.Close();

                    return false;

                default:
                    return false;
            }
        }
    }
}
