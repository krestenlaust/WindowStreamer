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
        private IPAddress _serverIP;
        private int _metastreamPort;
        private int _videostreamPort;
        private Size _videoResolution;
        private UdpClient _videoClient;
        private TcpClient _metaClient;
        private NetworkStream _metaStream;
        private Task _tcpLoop;
        private Size _formToPanelSize;
        

        public WindowDisplay()
        {
            InitializeComponent();
        }

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            _formToPanelSize = Size.Subtract(this.Size, displayArea.Size);

            _metastreamPort = Constants.MetaStreamPort;

            _metaClient = new TcpClient();
            _videoClient = new UdpClient();

        }

        private async Task ListenForUdp(IPAddress ip)
        {
            Log("Method: ListenForUdp");
            try
            {
                _videoClient = new UdpClient(_videostreamPort);
            }
            catch (Exception e)
            {
                Log("Exception: " + e.ToString());
            }
            IPEndPoint VideoStreamEndPoint = new IPEndPoint(ip, _videostreamPort);
            
            // Notify server that we are ready to start recieving image frames.
            //NetworkStream dataStream = MetaClient.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes(((int)MetaHeader.UDPReady).ToString() + Constants.ParameterSeparator +
                Constants.FramerateCap);

            External.PadArray(ref bytes, Constants.MetaFrameLength);
            await _metaStream.WriteAsync(bytes, 0, bytes.Length);
            

            while (_metaClient.Connected)
            {
                Log("Awaiting videostream bytes");
                byte[] recv_bytes = _videoClient.Receive(ref VideoStreamEndPoint);
                UpdateFrame(recv_bytes.Select(x => (int)x).ToArray());
                //await Task.Delay(Math.Max((int)(1000 / Constants.FramerateCap), 0));
            }
        }

        private void RecievedVideoFrame(IAsyncResult res)
        {
            IPEndPoint EndPoint = new IPEndPoint(_serverIP, _videostreamPort);
            byte[] recieved = _videoClient.EndReceive(res, ref EndPoint);

            Log("Method: RecievedVideoFrame");

            _videoClient.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
        }

        private async Task InitialMetaFrame()
        {
            Log("Method: InitialMetaFrame");
            
            while (!_metaStream.DataAvailable) {}

            Log("Data available");

            byte[] buffer = new byte[Constants.MetaFrameLength];
            await _metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

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
                IPEndPoint ipEndPoint = _metaClient.Client.RemoteEndPoint as IPEndPoint;

                UpdateResolution(new Size(
                    int.Parse(handshake[2].Split(Constants.SingleSeparator)[0]),
                    int.Parse(handshake[2].Split(Constants.SingleSeparator)[1])
                    ));

                _videostreamPort = int.Parse(handshake[3]);

                _videoClient.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);

                _tcpLoop = Task.Run(async () =>
                {
                    while (_metaClient.Connected)
                    {
                        if (_metaStream.DataAvailable)
                        {
                            await MetaPacketReceivedAsync();
                        }
                    }
                    Log("Connection lost... or disconnected");
                });
                _tcpLoop.Start();
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
            await _metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

            string[] metapacket = Encoding.UTF8.GetString(buffer).Replace("\0","").Split(Constants.ParameterSeparator);

            switch ((MetaHeader)int.Parse(metapacket[0]))
            {
                case MetaHeader.ConnectionReply:
                    Log("Recieved connections reply");
                    break;
                case MetaHeader.ResolutionUpdate:
                    Log("Recieved resolution update");
                    _videoResolution.Width = int.Parse(metapacket[1].Split(Constants.SingleSeparator)[0]);
                    _videoResolution.Height = int.Parse(metapacket[1].Split(Constants.SingleSeparator)[1]);
                    break;
                case MetaHeader.UDPReady:
                    Log("Recieved udp ready");
                    //VideoStream = new UdpClient(Constants.VideoStreamPort);
                    //IPEndPoint ip = MetaStream.Client.RemoteEndPoint as IPEndPoint;
                    //VideoStream.Connect(ip.Address, serverInfo.VideostreamPort);
                    break;
            }
        }

        private async Task ConnectToServerAsync(IPAddress serverIP, int port)
        {
            if (_metaClient.Connected)
            {
                Log($"Disconnecting {serverIP}:{port}");
                _metaClient.Close();
                _metaClient = new TcpClient();
            }

            Log($"Connecting to {serverIP}:{port}...");

            await _metaClient.ConnectAsync(serverIP, port);
            Log("Connected");

            _metaStream = _metaClient.GetStream();

            Log($"Awaiting response from {serverIP}");
            await InitialMetaFrame();
        }

        private void UpdateFrame(int[] frame)
        {
            Log("Method: UpdateFrame");
            DirectBitmap bmp = new DirectBitmap(_videoResolution.Width, _videoResolution.Height)
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
                    Task connectTask = new Task(async () =>
                    {
                        await ConnectToServerAsync(ConnectDialog.TargetIPAddress, ConnectDialog.TargetPort);
                    });
                    connectTask.Start();
                }
            }
        }

        private void UpdateResolution(Size resolution)
        {
            _videoResolution.Width = resolution.Width;
            _videoResolution.Height = resolution.Height;
            this.Width = _videoResolution.Width;
            this.Height = _videoResolution.Height;
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e) => ResizeToFit();
        private void ResizeToFit() => ResizeDisplayArea(_videoResolution);
        private void ResizeDisplayArea(Size size) => this.Size = Size.Add(_formToPanelSize, size);

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
