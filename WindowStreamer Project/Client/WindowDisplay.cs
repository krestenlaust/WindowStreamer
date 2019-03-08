using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using Shared;
using Shared.Protocol;

namespace Client
{
    public partial class WindowDisplay : Form
    {
        private IPAddress ServerIP;
        private int ServerPort;
        private UdpClient VideoStream;
        private TcpClient MetaStream;
        private Size VideoResolution;
        private Task TcpLoop;
        private Size FormToPanelSize;

        public WindowDisplay()
        {
            InitializeComponent();
        }

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            FormToPanelSize = Size.Subtract(this.Size, displayArea.Size);

            ServerPort = Constants.MetaStreamPort;

            MetaStream = new TcpClient();
            VideoStream = new UdpClient();
        }

        private async Task ListenForUdp(IPAddress ip)
        {
            VideoStream = new UdpClient(Constants.VideoStreamPort);
            IPEndPoint VideoStreamEndPoint = new IPEndPoint(ip, Constants.VideoStreamPort);

            if (MetaStream.Connected)
            {
                NetworkStream dataStream = MetaStream.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(MetaHeader.UDPStream.ToString());

                await dataStream.WriteAsync(bytes, 0, bytes.Length);
            }

            while (true)
            {
                byte[] bytes = VideoStream.Receive(ref VideoStreamEndPoint);
                UpdateFrame(bytes.Select(x => (int)x).ToArray());
                await Task.Delay(10);
            }
        }

        private void RecievedVideoFrame(IAsyncResult res)
        {
            IPEndPoint EndPoint = new IPEndPoint(ServerIP, Constants.VideoStreamPort);
            byte[] recieved = VideoStream.EndReceive(res, ref EndPoint);

            VideoStream.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
        }

        private void InitialMetaFrame(IAsyncResult res)
        {
            if (MetaStream.Connected)
            {
                NetworkStream dataStream = MetaStream.GetStream();
                byte[] buffer = new byte[30];
                dataStream.Read(buffer, 0, 30);

                string[] handshakeString = Encoding.UTF8.GetString(buffer).Split(',');

                if (handshakeString[1] == "1")
                {
                    Log("Connection request accepted, awaiting handshake finish...");
                    IPEndPoint ipEndPoint = MetaStream.Client.RemoteEndPoint as IPEndPoint;

                    VideoResolution.Width = Int32.Parse(handshakeString[2].Split('.')[0]);
                    VideoResolution.Height = Int32.Parse(handshakeString[2].Split('.')[1]);

                    ListenForUdp(ipEndPoint.Address);

                    TcpLoop = Task.Run(async () =>
                    {
                        while (MetaStream.Connected)
                        {
                            await MetaPacketReceivedAsync();
                        }
                        Log("Connection lost... or disconnected");
                        TcpLoop.Dispose();
                    });
                }
                else
                {
                    Log("Denied... :(");
                }
            }
            else
            {
                Log("Connection lost... or disconnected");
            }
        }

        private async Task MetaPacketReceivedAsync()
        {
            NetworkStream dataStream = MetaStream.GetStream();
            byte[] buffer = new byte[Constants.MetaFrameLength];
            await dataStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

            string[] metapacket = Encoding.UTF8.GetString(buffer).Split(',');

            switch ((MetaHeader)int.Parse(metapacket[0]))
            {
                case MetaHeader.ConnectionReply:
                    Log("Recieved connections reply");
                    break;
                case MetaHeader.ResolutionUpdate:
                    VideoResolution.Width = int.Parse(metapacket[1].Split('.')[0]);
                    VideoResolution.Height = int.Parse(metapacket[1].Split('.')[1]);
                    break;
            }
        }

        private bool Connect()
        {
            Log($"Connecting to {ServerIP}:{ServerPort}...");
            /*
            IPAddress ip;
            if (!IPAddress.TryParse(ipaddress, out ip))
            {
                return false;
            }*/

            if (MetaStream.Connected)
            {
                MetaStream.Close();
                MetaStream = new TcpClient();
            }

            VideoStream = new UdpClient();

            //await MetaStream.ConnectAsync(ip, Constants.MetaStreamPort);
            MetaStream.BeginConnect(ServerIP, ServerPort, new AsyncCallback(InitialMetaFrame), null);
            Log($"Awaiting response from {ServerIP}");

            //VideoStream.Connect(ServerIP, Constants.VideoStreamPort);
            //VideoStream.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
            return true;
        }

        private void UpdateFrame(int[] frame)
        {
            DirectBitmap bmp = new DirectBitmap(VideoResolution.Width, VideoResolution.Height)
            {
                Bits = frame
            };

            displayArea.Image = bmp.Bitmap;
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            using (var ConnectDialog = new ConnectWindow())
            {
                if (ConnectDialog.ShowDialog() == DialogResult.OK)
                {
                    ServerIP = ConnectDialog.TargetIPAddress;
                    ServerPort = ConnectDialog.TargetPort;
                    Connect();
                }
            }
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e) => ResizeToFit();
        private void ResizeToFit() => ResizeDisplayArea(VideoResolution);
        private void ResizeDisplayArea(Size size) => this.Size = Size.Add(FormToPanelSize, size);

        private void Log(object stdout)
        {
            toolStripStatusLabelLatest.Text = stdout.ToString();
        }
    }
}
