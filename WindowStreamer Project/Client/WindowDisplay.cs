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
using System.Runtime.InteropServices;

namespace Client
{

    public partial class WindowDisplay : Form
    {
        private IPAddress ServerIP;
        private int MetastreamPort;
        private int VideostreamPort;
        private Size VideoResolution;
        private UdpClient VideoStream;
        private TcpClient MetaStream;
        private Task TcpLoop;
        private Size FormToPanelSize;
        

        public WindowDisplay()
        {
            InitializeComponent();
        }

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            FormToPanelSize = Size.Subtract(this.Size, displayArea.Size);

            //ServerPort = Constants.MetaStreamPort;
            MetastreamPort = Constants.MetaStreamPort;

            MetaStream = new TcpClient();
            VideoStream = new UdpClient();

        }

        private async Task ListenForUdp(IPAddress ip)
        {
            Log("Method: ListenForUdp");
            VideoStream = new UdpClient(VideostreamPort);
            IPEndPoint VideoStreamEndPoint = new IPEndPoint(ip, VideostreamPort);

            if (MetaStream.Connected)
            {
                NetworkStream dataStream = MetaStream.GetStream();
                byte[] bytes = Encoding.UTF8.GetBytes(
                    ((int)MetaHeader.UDPReady).ToString() + Constants.ParameterSeparator +
                    Constants.FramerateCap);

                External.PadArray(ref bytes, Constants.MetaFrameLength);
                await dataStream.WriteAsync(bytes, 0, bytes.Length);
            }

            while (MetaStream.Connected)
            {
                byte[] bytes = VideoStream.Receive(ref VideoStreamEndPoint);
                UpdateFrame(bytes.Select(x => (int)x).ToArray());
                //await Task.Delay(Math.Max((int)(1000 / Constants.FramerateCap), 0));
            }
        }

        private void RecievedVideoFrame(IAsyncResult res)
        {
            Log("Method: RecievedVideoFrame");
            IPEndPoint EndPoint = new IPEndPoint(ServerIP, VideostreamPort);
            byte[] recieved = VideoStream.EndReceive(res, ref EndPoint);

            VideoStream.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
        }

        private async Task InitialMetaFrame()//IAsyncResult res)
        {
            Log("Method: InitialMetaFrame");
            if (!MetaStream.Connected)
            {
                Log("Connection lost... or disconnected");
                return;
            }

            NetworkStream dataStream = MetaStream.GetStream();
            
            while (!dataStream.DataAvailable) {}

            Log("Data available");

            byte[] buffer = new byte[Constants.MetaFrameLength];
            dataStream.Read(buffer, 0, Constants.MetaFrameLength);

            string handshakeString = Encoding.UTF8.GetString(buffer).Replace("\0", "");
            string[] handshake = handshakeString.Split(Constants.ParameterSeparator);
            Log($"Handshake:[{handshakeString}]");

            if (handshake[0] != ((int)MetaHeader.ConnectionReply).ToString())
            {
                InitialMetaFrame();
                return;
            }

            if (handshake[1] == "1")
            {
                Log("Connection request accepted, awaiting handshake finish...");
                IPEndPoint ipEndPoint = MetaStream.Client.RemoteEndPoint as IPEndPoint;

                //VideoResolution.Width = Int32.Parse(handshake[2].Split(Constants.SingleSeparator)[0]);
                //VideoResolution.Height = Int32.Parse(handshake[2].Split(Constants.SingleSeparator)[1]);
                UpdateResolution(new Size(
                    Int32.Parse(handshake[2].Split(Constants.SingleSeparator)[0]),
                    Int32.Parse(handshake[2].Split(Constants.SingleSeparator)[1])
                    ));

                VideostreamPort = Int32.Parse(handshake[3]);

                ListenForUdp(ipEndPoint.Address);

                TcpLoop = Task.Run(async () =>
                {
                    while (MetaStream.Connected)
                    {
                        if (MetaStream.GetStream().DataAvailable)
                        {
                            await MetaPacketReceivedAsync();
                        }
                    }
                    Log("Connection lost... or disconnected");
                    //TcpLoop.Dispose();
                });
                Log("Handshake end");
            }
            else if (handshake[1] == "0")
            {
                Log("Denied :(");
            }
        }

        private async Task MetaPacketReceivedAsync()
        {
            Log("Method: MetaPacketReceivedAsync");
            NetworkStream dataStream = MetaStream.GetStream();
            byte[] buffer = new byte[Constants.MetaFrameLength];
            await dataStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

            string[] metapacket = Encoding.UTF8.GetString(buffer).Replace("\0","").Split(Constants.ParameterSeparator);

            switch ((MetaHeader)int.Parse(metapacket[0]))
            {
                case MetaHeader.ConnectionReply:
                    Log("Recieved connections reply");
                    break;
                case MetaHeader.ResolutionUpdate:
                    Log("Recieved resolution update");
                    VideoResolution.Width = int.Parse(metapacket[1].Split(Constants.SingleSeparator)[0]);
                    VideoResolution.Height = int.Parse(metapacket[1].Split(Constants.SingleSeparator)[1]);
                    break;
                case MetaHeader.UDPReady:
                    Log("Recieved udp ready");
                    //VideoStream = new UdpClient(Constants.VideoStreamPort);
                    //IPEndPoint ip = MetaStream.Client.RemoteEndPoint as IPEndPoint;
                    //VideoStream.Connect(ip.Address, serverInfo.VideostreamPort);
                    break;
            }
        }

        private async Task ConnectAsync()
        {
            Log($"Connecting to {ServerIP}:{MetastreamPort}...");
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

            //VideoStream = new UdpClient();

            //await MetaStream.ConnectAsync(ip, Constants.MetaStreamPort);
            //MetaStream.BeginConnect(serverInfo.ServerIP, ServerPort, new AsyncCallback(InitialMetaFrame), null);
            Log($"Awaiting response from {ServerIP}");

            await MetaStream.ConnectAsync(ServerIP, MetastreamPort);
            if (MetaStream.Connected)
            {
                Log("Connected");
                await InitialMetaFrame();
            }
            else
            {
                Log("Disconnected");
            }
        }

        private void UpdateFrame(int[] frame)
        {
            Log("Method: UpdateFrame");
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
                    MetastreamPort = ConnectDialog.TargetPort;
                    ConnectAsync();
                }
            }
        }

        private void UpdateResolution(Size resolution)
        {
            VideoResolution.Width = resolution.Width;
            VideoResolution.Height = resolution.Height;
            this.Width = VideoResolution.Width;
            this.Height = VideoResolution.Height;
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e) => ResizeToFit();
        private void ResizeToFit() => ResizeDisplayArea(VideoResolution);
        private void ResizeDisplayArea(Size size) => this.Size = Size.Add(FormToPanelSize, size);

        private void Log(object stdout)
        {
            toolStripStatusLabelLatest.Text = stdout.ToString();
            System.Diagnostics.Debug.WriteLine("[Client] "+stdout);
        }

        //Click through
        /*[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;

        public void PerformClick()
        {
            uint X = (uint)Cursor.Position.X;
            uint Y = (uint)Cursor.Position.Y;
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }*/
    }
}
