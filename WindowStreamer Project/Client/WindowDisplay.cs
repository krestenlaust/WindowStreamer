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
using Shared.Networking.Protocol;
using System.Runtime.InteropServices;

namespace Client
{

    public partial class WindowDisplay : Form
    {
        private IPAddress ServerIP;
        private int MetastreamPort;
        private int VideostreamPort;
        private Size VideoResolution;
        private UdpClient VideoClient;
        private TcpClient MetaClient;
        private NetworkStream MetaStream;
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

            MetaClient = new TcpClient();
            VideoClient = new UdpClient();

        }

        private async Task ListenForUdp(IPAddress ip)
        {
            Log("Method: ListenForUdp");
            try
            {
                VideoClient = new UdpClient(VideostreamPort);
            }
            catch (Exception e)
            {
                Log("Exception: " + e.ToString());
            }
            IPEndPoint VideoStreamEndPoint = new IPEndPoint(ip, VideostreamPort);
            
            // Notify server that we are ready to start recieving image frames.
            //NetworkStream dataStream = MetaClient.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes(((int)MetaHeader.UDPReady).ToString() + Constants.ParameterSeparator +
                Constants.FramerateCap);

            External.PadArray(ref bytes, Constants.MetaFrameLength);
            await MetaStream.WriteAsync(bytes, 0, bytes.Length);
            

            while (MetaClient.Connected)
            {
                Log("Awaiting videostream bytes");
                byte[] recv_bytes = VideoClient.Receive(ref VideoStreamEndPoint);
                UpdateFrame(recv_bytes.Select(x => (int)x).ToArray());
                //await Task.Delay(Math.Max((int)(1000 / Constants.FramerateCap), 0));
            }
        }

        private void RecievedVideoFrame(IAsyncResult res)
        {
            Log("Method: RecievedVideoFrame");
            IPEndPoint EndPoint = new IPEndPoint(ServerIP, VideostreamPort);
            byte[] recieved = VideoClient.EndReceive(res, ref EndPoint);

            VideoClient.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
        }

        private async Task InitialMetaFrame()//IAsyncResult res)
        {
            Log("Method: InitialMetaFrame");
            // NOTE: Remove this, check is made in previus method.
            if (!MetaClient.Connected)
            {
                Log("Connection lost... or disconnected");
                return;
            }

            //NetworkStream dataStream = MetaClient.GetStream();
            
            while (!MetaStream.DataAvailable) {}

            Log("Data available");

            byte[] buffer = new byte[Constants.MetaFrameLength];
            MetaStream.Read(buffer, 0, Constants.MetaFrameLength);

            string handshakeString = Encoding.UTF8.GetString(buffer).Replace("\0", "");
            string[] handshake = handshakeString.Split(Constants.ParameterSeparator);
            Log($"Handshake:[{handshakeString}]");

            if (handshake[0] != ((int)MetaHeader.ConnectionReply).ToString())
            {
                await InitialMetaFrame();
                return;
            }

            if (handshake[1] == "1")
            {
                Log("Connection request accepted, awaiting handshake finish...");
                IPEndPoint ipEndPoint = MetaClient.Client.RemoteEndPoint as IPEndPoint;

                UpdateResolution(new Size(
                    Int32.Parse(handshake[2].Split(Constants.SingleSeparator)[0]),
                    Int32.Parse(handshake[2].Split(Constants.SingleSeparator)[1])
                    ));

                VideostreamPort = Int32.Parse(handshake[3]);

                try
                {
                    VideoClient.BeginReceive(RecievedVideoFrame, VideoClient);

                }catch(Exception e)
                {
                    Log(e);
                }
                //ListenForUdp(ipEndPoint.Address);

                TcpLoop = Task.Run(async () =>
                {
                    while (MetaClient.Connected)
                    {
                        if (MetaStream.DataAvailable)
                        {
                            await MetaPacketReceivedAsync();
                        }
                    }
                    Log("Connection lost... or disconnected");
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
            //NetworkStream dataStream = MetaClient.GetStream();
            byte[] buffer = new byte[Constants.MetaFrameLength];
            await MetaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

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

            if (MetaClient.Connected)
            {
                MetaClient.Close();
                MetaClient = new TcpClient();
            }

            Log($"Awaiting response from {ServerIP}");
            await MetaClient.ConnectAsync(ServerIP, MetastreamPort);
            Log("Connected");
            MetaStream = MetaClient.GetStream();
            await InitialMetaFrame();
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
            System.Diagnostics.Debug.WriteLine("[Client] " + stdout);
        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
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
