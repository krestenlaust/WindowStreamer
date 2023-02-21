using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Protocol;
using Shared;

#nullable enable

namespace Client
{
    public class WindowClient : IDisposable
    {
        private readonly int metastreamPort;
        private readonly IPAddress serverIP;
        private readonly Action<string> log;
        private int? videostreamPort;

        private UdpClient videoClient;
        private TcpClient metaClient;
        private NetworkStream metaStream;
        private bool disposedValue;

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

        public event Action<byte[]> VideoframeRecieved;

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

        private void RecieveDatagram(IAsyncResult res)
        {
            IPEndPoint endPoint = new IPEndPoint(serverIP, videostreamPort!.Value);
            byte[] recieved = videoClient.EndReceive(res, ref endPoint!);

            VideoframeRecieved?.Invoke(recieved);
            videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);
        }

        private async Task MetastreamLoop()
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
                        log($"Handshake recieved");

                        if (Parse.ConnectionReply(metapacket, out bool accepted, out Size resolution, out int videoPort))
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
                        Parse.ResolutionChange(metapacket, out Size newResolution);

                        ResolutionChanged?.Invoke(newResolution);

                        break;
                    default:
                        log($"Recieved this: {metapacket[0]}");
                        break;
                }
            }

            log("Connection lost... or disconnected");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WindowClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
