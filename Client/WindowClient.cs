using System;
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
        /// Event called when a complete frame has been recieved.
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
            byte[] recieved = videoClient.EndReceive(res, ref endPoint!);

            NewFrame?.Invoke(recieved);
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
