using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Shared;

namespace Client
{
    public class WindowClient : IDisposable
    {
        readonly int metastreamPort;
        readonly IPAddress serverIP;
        readonly Action<string> log;

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
        public WindowClient(IPAddress serverIP, int metastreamPort, Action<string> logger)
        {
            this.serverIP = serverIP;
            this.metastreamPort = metastreamPort;
            log = logger;
        }

        /// <summary>
        /// Event called when a complete frame has been received.
        /// </summary>
        public event Action<byte[]> NewFrame;

        /// <summary>
        /// Event called when the resolution is changed.
        /// </summary>
        public event Action<Size> ResolutionChanged;

        public async Task ConnectToServerAsync()
        {
            if (metaClient?.Connected == true)
            {
                log($"Disconnecting {serverIP}:{metastreamPort}");
                metaClient.Close();
            }

            metaClient = new TcpClient();

            log($"Connecting to {serverIP}:{metastreamPort}...");

            try
            {
                await metaClient.ConnectAsync(serverIP, metastreamPort);
            }
            catch (SocketException)
            {
                log("Connection unsuccessful.");
                return;
            }

            metaStream = metaClient.GetStream();

            log($"Awaiting response from {serverIP}");

            await Task.Run(MetastreamLoop);
        }

        public void Dispose()
        {
            videoClient?.Dispose();
            metaClient?.Dispose();
            metaStream?.Dispose();
        }

        const int packetCount = 32;
        byte[] wholePacket = null;
        int chunksReceived = 0;
        // Used for debugging
        int packetsReceived = 0;

        void RecieveDatagram(IAsyncResult res)
        {
            var endPoint = new IPEndPoint(serverIP, videostreamPort!.Value);
            byte[] received = videoClient.EndReceive(res, ref endPoint!);
            packetsReceived++;

            ushort packetIndex = BitConverter.ToUInt16(received, 0);
            int totalSize = BitConverter.ToInt32(received, sizeof(ushort));
            int chunkSize = ((totalSize - 1) / packetCount) + 1;

            if (wholePacket is null)
            {
                wholePacket = new byte[totalSize];
            }

            Buffer.BlockCopy(received, sizeof(ushort) + sizeof(int), wholePacket, chunkSize * packetIndex, received.Length - (sizeof(ushort) + sizeof(int)));

            // Packet is assembled
            if (chunksReceived == packetCount - 1)
            {
                NewFrame?.Invoke(wholePacket);
                wholePacket = null;
                chunksReceived = -1;
            }

            chunksReceived++;

            videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);
        }

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
                        log($"Handshake received");

                        if (Parse.TryParseConnectionReply(metapacket, out bool accepted, out Size resolution, out int videoPort))
                        {
                            videostreamPort = videoPort;
                            ResolutionChanged?.Invoke(resolution);
                        }
                        else
                        {
                            log("Failed to parse packet");
                            continue;
                        }

                        if (!accepted)
                        {
                            // TODO: implement some connection ended logic.
                            log("Connection request denied :(");
                            return;
                        }

                        log("Connection request accepted, awaiting handshake finish...");
                        IPEndPoint ipEndPoint = (IPEndPoint)metaClient.Client.RemoteEndPoint!;

                        videoClient = new UdpClient(videoPort);
                        videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);

                        byte[] udpReady = Send.UDPReady(DefaultValues.FramerateCap);
                        metaStream.Write(udpReady, 0, udpReady.Length);

                        handshakeFinished = true;

                        break;
                    case ServerPacketHeader.ResolutionUpdate when handshakeFinished:
                        log("Recieved resolution update");
                        Parse.TryParseResolutionChange(metapacket, out Size newResolution);

                        ResolutionChanged?.Invoke(newResolution);

                        break;
                    default:
                        log($"Recieved this: {metapacket[0]}");
                        break;
                }
            }

            log("Connection lost... or disconnected");
        }
    }
}
