using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Shared;
using Shared.Networking;
using System.IO;
using System.Drawing.Imaging;

namespace Server
{
    public partial class WindowCapture : Form
    {
        public IntPtr TargetWindowHandle { get; private set; } = IntPtr.Zero;

        //private UdpClient videoStream;
        private IPEndPoint _clientEndpoint;
        private UdpClient _videoClient;
        private TcpClient _metaClient = new TcpClient(AddressFamily.InterNetwork);
        private NetworkStream _metaStream;
        private TcpListener _clientListener;
        private IPAddress _acceptedAddress = IPAddress.Any;
        private Size _videoResolution;
        private bool _fullscreen = false;
        private bool _streamVideo = true;
        private Task _tcpLoop = null;
        private Size _lastResolution;
        private Size _fullscreenSize = new Size
        {
            Width = 400,
            Height = 100
        };
        private Point _captureAreaTopLeft;
        private Cursor _applicationSelectorCursor;

        public WindowCapture()
        {
            InitializeComponent();
        }

        private void WindowCapture_Load(object sender, EventArgs e)
        {
            _applicationSelectorCursor = Cursors.Hand;

            TransparencyKey = Color.Orange;
            captureArea.BackColor = Color.Orange;
            UpdateResolutionVariables();

            toolStripTextBoxTargetPort.Text = Constants.MetaStreamPort.ToString();
        }

        private Bitmap GetScreenPicture(int x, int y, int width, int height)
        {
            // TODO: Debug funktionen for at oversætte skærm koordinaterne til pixels.
            /*Rectangle screen = Rectangle.Empty;
            this.Invoke((MethodInvoker)delegate
            {
                screen = Screen.FromControl(this).WorkingArea;
            });
            
            position.X = position.X / screen.Width; // screen.Width: 1920
            position.Y = position.Y / screen.Height; // screen.Height: 1080
            */

            Rectangle rect = new Rectangle(x, y, width, height);
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            return bmp;
        }

        private void SendPicture(UdpClient client)
        {
            if (!client.Client.Connected || !_streamVideo)
                return;

            Bitmap bmp = GetScreenPicture(captureArea.Location.X + Location.X, captureArea.Location.Y + Location.Y, _videoResolution.Width, _videoResolution.Height);
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                bytes = stream.ToArray();
                Log(bytes.Length);
            }

            client.Send(bytes, bytes.Length);
        }

        private void ConnectVideoStream()
        {
            _videoClient.Connect(_clientEndpoint.Address, Constants.VideoStreamPort);
            _videoClient.DontFragment = false;
        }

        private async void BeginStreamLoop()
        {
            while (_streamVideo)
            {
                SendPicture(_videoClient);

                await Task.Delay(50);
            }
        }

        private async Task StartServerAsync()
        {
            _clientListener?.Stop();
            _clientListener = new TcpListener(_acceptedAddress, Constants.MetaStreamPort);
            _clientListener.Start();
            Log($"Server started {_acceptedAddress}:{Constants.MetaStreamPort}");

            _metaClient = await _clientListener.AcceptTcpClientAsync();
            _metaStream = _metaClient.GetStream();
            Log("Connection recieved...");

            _tcpLoop = Task.Run(() =>
            {
                while (_metaClient.Connected)
                {
                    if (_metaClient.Available >= Constants.MetaFrameLength)
                    {
                        byte[] buffer = new byte[Constants.MetaFrameLength];
                        _metaStream.Read(buffer, 0, Constants.MetaFrameLength);

                        string[] metapacket = Encoding.UTF8.GetString(buffer).Replace("\0", "").Split(Constants.ParameterSeparator);

                        switch ((ClientPacketHeader)int.Parse(metapacket[0]))
                        {
                            case ClientPacketHeader.Key:
                                Log($"Recieved key: {metapacket[1]}");
                                break;
                            case ClientPacketHeader.UDPReady:
                                Log("Udp ready!");
                                ConnectVideoStream();
                                BeginStreamLoop();
                                break;
                            default:
                                Log($"Recived this: {metapacket[0]}");
                                break;
                        }
                    }
                }
                Log("Connection lost... or disconnected(tcp loop)");
            });

            await HandshakeAsync();
        }

        /// <summary>
        /// Make handshake and begin listening loop.
        /// </summary>
        /// <returns></returns>
        private async Task HandshakeAsync()
        {
            _clientEndpoint = _metaClient.Client.RemoteEndPoint as IPEndPoint;
            DialogResult diaglogResult = DialogResult.Ignore;

            await Task.Run(() =>
            {
                Log("Inbound connection, awaiting action...");
                ConnectionPrompt prompt = new ConnectionPrompt();
                prompt.IPAddress = _clientEndpoint.Address.ToString();
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.TopMost = true;
                diaglogResult = prompt.ShowDialog();
            });

            switch (diaglogResult)
            {
                case DialogResult.Abort: //Block
                    Log("Blocked the following IP Address: " + _clientEndpoint.Address);

                    BlockIPAddress(_clientEndpoint.Address);
                    _metaClient.Close();

                    await StartServerAsync();
                    return;

                case DialogResult.Ignore: //Ignore
                    Log("Ignored connection");

                    _metaClient.Close();

                    await StartServerAsync();
                    return;

                case DialogResult.Yes: //Accept
                    Log("Accepting connection");

                    _videoClient = new UdpClient();
                    _streamVideo = true;
                    Send.ConnectionReply(_metaStream, true, _videoResolution, Constants.VideoStreamPort);
                    

                    //videoStream = new UdpClient(Constants.VideoStreamPort); //JIWJDIAJWDJ
                    break;
                case DialogResult.No: //Deny
                    Send.ConnectionReply(_metaStream, false, Size.Empty, 0);

                    Log($"Told {_clientEndpoint.Address} to try again another day :)");
                    _metaClient.Close();

                    await StartServerAsync();
                    return;
                default:
                    Log("DialogResult returned unknown value");
                    return;
            }
        }

        private void BlockIPAddress(IPAddress ip)
        {
            Console.WriteLine("Blocking IP: " + ip.ToString());
        }

        private void WindowCapture_Resize(object sender, EventArgs e) => UpdateResolutionVariables();

        private void toolStripButtonFocusOnWindow_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }

        private async void toolStripButtonConnect_ClickAsync(object sender, EventArgs e)
        {
            if (toolStripTextBoxAcceptableHost.Text == "")
            {
                _acceptedAddress = IPAddress.Any;
            }
            else if (!IPAddress.TryParse(toolStripTextBoxAcceptableHost.Text, out _acceptedAddress))
            {
                MessageBox.Show("IP not valid", "Error");
                return;
            }
            
            await StartServerAsync();
        }

        private void toolStripButtonApplicationPicker_MouseHover(object sender, EventArgs e)
        {

        }

        private void toolStripButtonApplicationPicker_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonApplicationSelector_Click(object sender, EventArgs e)
        {
            toolStripButtonApplicationSelector.ToolTipText = "Click here";
        }

        /// <summary>
        /// Notifies client of resolution change.
        /// </summary>
        private void NotifyResolutionChange()
        {
            if (_metaClient?.Connected == false)
            {
                Log("Not connected...");
                return;
            }

            Send.ResolutionChange(_metaStream, _videoResolution);
        }

        private void UpdateResolutionVariables()
        {
            if (_fullscreen)
            {
                _videoResolution.Height = Screen.FromControl(this).Bounds.Height;
                _videoResolution.Width = Screen.FromControl(this).Bounds.Width;
                _captureAreaTopLeft = Screen.FromControl(this).Bounds.Location;
            } else
            {
                _videoResolution.Height = captureArea.Height;
                _videoResolution.Width = captureArea.Width;
                _captureAreaTopLeft = captureArea.Bounds.Location;
            }

            toolStripStatusLabelResolution.Text = _videoResolution.Width.ToString() + "x" + _videoResolution.Height.ToString();

            NotifyResolutionChange();
        }

        private void Log(object stdout, [CallerLineNumber] int line=0)
        {
            string time = DateTime.Now.ToString("mm:ss:ffff");

            if (this.IsHandleCreated)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabelLatest.Text = "[" + time + "] " + stdout.ToString();
                });
            }
            Debug.WriteLine($"[{time}][{line}][Server] {stdout}");
        }

        /*
        private void StartLogWindow()
        {
            if (Other.Statics.logWindow != null)
            {
                
            }
            LogWindow lw = new LogWindow();
            lw.FormClosed += new FormClosedEventHandler(LogWindowClosed);
        }*/

        private void LogWindowClosed(object sender, FormClosedEventArgs e)
        {
            LogWindow lw = sender as LogWindow;
            Console.SetOut(lw.sw);
        }

        private void toolStripButtonDebug1_Click(object sender, EventArgs e)
        {

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    _fullscreen = true;
                    _lastResolution = this.Size;
                    this.Size = _fullscreenSize;
                    UpdateResolutionVariables();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    _fullscreen = false;
                    this.Size = _lastResolution;
                    UpdateResolutionVariables();
                }
            }
            base.WndProc(ref m);
        }
    }

    public static class WindowActions
    {
        public static void ClickOnPoint(IntPtr handle, Point point, int mouseButton = 0)
        {
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Handle not attached");
                return;
            }

            Point cursorPosition = Cursor.Position;

            NativeMethods.ClientToScreen(handle, ref point);

            Cursor.Position = new Point(point.X, point.Y);

            NativeMethods.INPUT inputMouseDown = new NativeMethods.INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            NativeMethods.INPUT inputMouseUp = new NativeMethods.INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new NativeMethods.INPUT[] { inputMouseDown, inputMouseUp };
            NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(NativeMethods.INPUT)));

            /// return mouse 
            Cursor.Position = cursorPosition;
        }

        public static void InputKey(IntPtr handle, Keys key)
        {

        }
    }
}