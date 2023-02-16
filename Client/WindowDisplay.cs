using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;
using Shared.Networking;

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
        private MemoryStream _bitmapStream;

        public WindowDisplay()
        {
            InitializeComponent();
        }

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            _formToPanelSize = Size.Subtract(this.Size, displayArea.Size);

            _metastreamPort = Constants.MetaStreamPort;

            _metaClient = new TcpClient();
        }

        private void UpdateFrame(byte[] frame)
        {
            Log("Updated frame");
            _bitmapStream?.Dispose();
            _bitmapStream = new MemoryStream(frame);

            displayArea.Image = new Bitmap(_bitmapStream);
        }

        private void RecieveDatagram(IAsyncResult res)
        {
            IPEndPoint endPoint = new IPEndPoint(_serverIP, _videostreamPort);

            byte[] recieved = _videoClient.EndReceive(res, ref endPoint);

            UpdateFrame(recieved);

            _videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);
        }

        private async Task InitialMetaFrame()
        {
            while (_metaClient.Available < Constants.MetaFrameLength)
            {
            }

            Log("Data available");

            byte[] buffer = new byte[Constants.MetaFrameLength];
            await _metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

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
                _videoResolution = resolution;
                _videostreamPort = videoPort;
                ResizeDisplayArea(_videoResolution);
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
            IPEndPoint ipEndPoint = _metaClient.Client.RemoteEndPoint as IPEndPoint;

            //SetResolution(resolution);

            _videoClient = new UdpClient(videoPort);
            _videoClient.BeginReceive(new AsyncCallback(RecieveDatagram), null);

            Send.UDPReady(_metaStream, Constants.FramerateCap);

            await Task.Run(async () =>
            {
                while (_metaClient.Connected)
                {
                    if (_metaClient.Available >= Constants.MetaFrameLength)
                    {
                        byte[] packet = new byte[Constants.MetaFrameLength];
                        await _metaStream.ReadAsync(packet, 0, Constants.MetaFrameLength);

                        string[] metapacket = Encoding.UTF8.GetString(packet).Replace("\0", string.Empty).Split(Constants.ParameterSeparator);

                        switch ((ServerPacketHeader)int.Parse(metapacket[0]))
                        {
                            case ServerPacketHeader.ResolutionUpdate:
                                Log("Recieved resolution update");
                                Parse.ResolutionChange(metapacket, out _videoResolution);

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
            if (_metaClient.Connected)
            {
                Log($"Disconnecting {_serverIP}:{_metastreamPort}");
                _metaClient.Close();
                _metaClient = new TcpClient();
            }

            Log($"Connecting to {_serverIP}:{_metastreamPort}...");

            try
            {
                await _metaClient.ConnectAsync(_serverIP, _metastreamPort);
            }
            catch (SocketException)
            {
                Log("Connection unsuccessful.");
                return;
            }

            Log("Connected");

            _metaStream = _metaClient.GetStream();

            Log($"Awaiting response from {_serverIP}");
            await InitialMetaFrame();
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            using (var connectDialog = new ConnectWindow())
            {
                if (connectDialog.ShowDialog() == DialogResult.OK)
                {
                    _serverIP = connectDialog.TargetIPAddress;
                    _metastreamPort = connectDialog.TargetPort;

                    Task.Run(async () =>
                    {
                        await ConnectToServerAsync();
                    });
                }
            }
        }

        private void SetResolution(Size resolution)
        {
            _videoResolution.Width = resolution.Width;
            _videoResolution.Height = resolution.Height;
            this.Width = _videoResolution.Width;
            this.Height = _videoResolution.Height;
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e)
        {
            if (toolStripButtonResizeToFit.Checked)
            {
                ResizeToFit();
            }
        }

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
    }
}
