using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;
using Protocol;

namespace Client
{
    public partial class WindowDisplay : Form
    {
        private IPAddress serverIP;
        private int metastreamPort;
        private int videostreamPort;
        private Size videoResolution;
        private UdpClient videoClient;
        private TcpClient metaClient;
        private NetworkStream metaStream;
        private Task tcpLoop;
        private Size formToPanelSize;
        private MemoryStream bitmapStream;

        public WindowDisplay()
        {
            InitializeComponent();
        }

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            formToPanelSize = Size.Subtract(this.Size, displayArea.Size);

            metastreamPort = DefaultValues.MetaStreamPort;

            metaClient = new TcpClient();
        }

        private void UpdateFrame(byte[] frame)
        {
            Log("Updated frame");
            bitmapStream?.Dispose();
            bitmapStream = new MemoryStream(frame);

            displayArea.Image = new Bitmap(bitmapStream);
        }

        private void RecieveDatagram(IAsyncResult res)
        {
            IPEndPoint endPoint = new IPEndPoint(serverIP, videostreamPort);

            byte[] recieved = videoClient.EndReceive(res, ref endPoint);

            UpdateFrame(recieved);

            videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);
        }

        private async Task InitialMetaFrame()
        {
            while (metaClient.Available < Constants.MetaFrameLength)
            {
            }

            Log("Data available");

            byte[] buffer = new byte[Constants.MetaFrameLength];
            await metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

            string handshakeString = Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty);
            Log($"Handshake:[{handshakeString}]");

            string[] handshake = handshakeString.Split(Constants.ParameterSeparator);

            if (handshake[0] != ((int)ServerPacketHeader.ConnectionReply).ToString())
            {
                Log("Expected handshake, got something else.");
                await InitialMetaFrame();
                return;
            }

            if (Parse.ConnectionReply(handshake, out bool accepted, out Size resolution, out int videoPort))
            {
                videoResolution = resolution;
                videostreamPort = videoPort;
                ResizeDisplayArea(videoResolution);
            }
            else
            {
                Log("Failed to parse packet");
                await InitialMetaFrame();
                return;
            }

            if (!accepted)
            {
                Log("Connection request denied :(");
                return;
            }

            Log("Connection request accepted, awaiting handshake finish...");
            IPEndPoint ipEndPoint = metaClient.Client.RemoteEndPoint as IPEndPoint;

            //SetResolution(resolution);

            videoClient = new UdpClient(videoPort);
            videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);

            Send.UDPReady(metaStream, DefaultValues.FramerateCap);

            await Task.Run(async () =>
            {
                while (metaClient.Connected)
                {
                    if (metaClient.Available >= Constants.MetaFrameLength)
                    {
                        byte[] packet = new byte[Constants.MetaFrameLength];
                        await metaStream.ReadAsync(packet, 0, Constants.MetaFrameLength);

                        string[] metapacket = Encoding.UTF8.GetString(packet).Replace("\0", string.Empty).Split(Constants.ParameterSeparator);

                        switch ((ServerPacketHeader)int.Parse(metapacket[0]))
                        {
                            case ServerPacketHeader.ResolutionUpdate:
                                Log("Recieved resolution update");
                                Parse.ResolutionChange(metapacket, out videoResolution);

                                if (toolStripButtonResizeToFit.Checked)
                                {
                                    ResizeToFit();
                                }

                                break;
                            default:
                                Log($"Recieved this: {metapacket[0]}");
                                break;
                        }
                    }
                }

                Log("Connection lost... or disconnected");
            });
        }

        private async Task ConnectToServerAsync()
        {
            if (metaClient.Connected)
            {
                Log($"Disconnecting {serverIP}:{metastreamPort}");
                metaClient.Close();
                metaClient = new TcpClient();
            }

            Log($"Connecting to {serverIP}:{metastreamPort}...");

            try
            {
                await metaClient.ConnectAsync(serverIP, metastreamPort);
            }
            catch (SocketException)
            {
                Log("Connection unsuccessful.");
                return;
            }

            Log("Connected");

            metaStream = metaClient.GetStream();

            Log($"Awaiting response from {serverIP}");
            await InitialMetaFrame();
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            using (var connectDialog = new ConnectWindow())
            {
                if (connectDialog.ShowDialog() == DialogResult.OK)
                {
                    serverIP = connectDialog.TargetIPAddress;
                    metastreamPort = connectDialog.TargetPort;

                    Task.Run(async () =>
                    {
                        await ConnectToServerAsync();
                    });
                }
            }
        }

        private void SetResolution(Size resolution)
        {
            videoResolution.Width = resolution.Width;
            videoResolution.Height = resolution.Height;
            this.Width = videoResolution.Width;
            this.Height = videoResolution.Height;
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e)
        {
            if (toolStripButtonResizeToFit.Checked)
            {
                ResizeToFit();
            }
        }

        private void ResizeToFit() => ResizeDisplayArea(videoResolution);

        private void ResizeDisplayArea(Size size) => this.Size = Size.Add(formToPanelSize, size);

        private void Log(object stdout)
        {
            toolStripStatusLabelLatest.Text = stdout.ToString();
            System.Diagnostics.Debug.WriteLine("[Client] " + stdout);
        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }
    }
}
