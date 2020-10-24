using System;
//using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Shared;
using Shared.Networking.Protocol;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Server
{
    public partial class WindowCapture : Form
    {
        public IntPtr TargetWindowHandle { get; private set; } = IntPtr.Zero;

        //private UdpClient videoStream;
        private IPEndPoint _clientEndpoint;
        private TcpClient _metaClient = new TcpClient(AddressFamily.InterNetwork);
        private NetworkStream _metaStream;
        private TcpListener _clientListener;
        private IPAddress _acceptedAddress = IPAddress.Any;
        private Size _videoResolution;
        private bool _fullscreen = false;
        private bool _streamVideo = true;
        private double _framesPerSecond = 10;
        private Task _tcpLoop = null;
        private Size _lastResolution;
        private Size _fullscreenSize = new Size
        {
            Width = 400,
            Height = 100
        };
        private long _lastSentFrame;
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
            UpdateResolution();

            toolStripTextBoxTargetPort.Text = Constants.MetaStreamPort.ToString();
        }
        
        private async Task NewFrame()
        {
            if (_metaClient.Connected && _streamVideo)
            {
                #region Work in progrss
                /*
                DirectBitmap bmp = new DirectBitmap(CaptureSize.Width, CaptureSize.Height);
                Graphics g = Graphics.FromImage(bmp.Bitmap);

                g.CopyFromScreen(CaptureAreaTopLeft, CaptureAreaTopLeft, CaptureSize);
                g.Dispose();*/

                //byte[] imagePayload = CompressImage(bmp);
                //LockBitmap lbmp = new LockBitmap(bmp);
                //byte[] imagePayload = lbmp.Pixels;

                //byte[] imagePayload;
                //imagePayload = bmp.ToByteArray(ImageFormat.Bmp);
                #endregion

                Bitmap bmp = new Bitmap(_videoResolution.Width, _videoResolution.Height);
                byte[] bytes = bmp.ToByteArray(System.Drawing.Imaging.ImageFormat.Bmp);
                //await videoStream.SendAsync(bytes, bytes.Length);
                Log("Sending frame");
                using (UdpClient sender = new UdpClient(0))
                {
                    await sender.SendAsync(bytes, bytes.Length, _clientEndpoint.Address.ToString(), Constants.VideoStreamPort);
                }
                Log("Sent frame");

                //await VideoStream.SendAsync(imagePayload, imagePayload.Length);
            }
        }

        private async Task StreamCycle()
        {
            if (_metaClient.Connected)
            {
                if (_framesPerSecond == 0)
                {
                    _lastSentFrame = External.TimeStamp();
                    await NewFrame();
                }
            }
            while (_metaClient.Connected)
            {
                if (External.TimeStamp() - _lastSentFrame > 1000 / _framesPerSecond)
                {
                    _lastSentFrame = External.TimeStamp();
                    await NewFrame();
                    Log("Fixed Framerate tick");
                }
            }
            Log("Metastream not connected");
        }

        private async Task BeginHandshakeAsync()
        {
            _clientEndpoint = _metaClient.Client.RemoteEndPoint as IPEndPoint;

            DialogResult diaglogResult = DialogResult.Ignore;


            await Task.Run(() =>
            {
                Log("Inbound connection, awaiting prompt return...");
                ConnectionPrompt prompt = new ConnectionPrompt();
                prompt.IPAddress = _clientEndpoint.Address.ToString();
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.TopMost = true;
                diaglogResult = prompt.ShowDialog();
            });
            Log("Prompt returned...");

            string handshakeString;

            switch (diaglogResult)
            {
                case DialogResult.Abort: //Block
                    BlockIPAddress(_clientEndpoint.Address);
                    Log("Blocked the following IP Address: " + _clientEndpoint.Address);
                    _metaClient.Close();

                    await StartServerAsync();
                    return;

                case DialogResult.Ignore: //Ignore
                    _metaClient.Close();
                    Log("Ignored connection");

                    await StartServerAsync();
                    return;

                case DialogResult.Yes: //Accept
                    handshakeString = ((int)MetaHeader.ConnectionReply).ToString() + 
                        Constants.ParameterSeparator + '1' + 
                        Constants.ParameterSeparator + _videoResolution.Width + Constants.SingleSeparator + _videoResolution.Height + 
                        Constants.ParameterSeparator + Constants.VideoStreamPort;

                    //videoStream = new UdpClient(Constants.VideoStreamPort); //JIWJDIAJWDJ
                    break;
                case DialogResult.No: //Deny
                    handshakeString = ((int)MetaHeader.ConnectionReply).ToString() + Constants.ParameterSeparator + '0';
                    break;
                default:
                    Log("DialogResult returned other than allowed values");
                    return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(handshakeString);
            External.PadArray(ref bytes, Constants.MetaFrameLength);

            await _metaStream.WriteAsync(bytes, 0, bytes.Length);
            Log($"Handshake: [{handshakeString}]");
            
            if (handshakeString.Split(',')[1] == "0")
            {
                Log($"Told {_clientEndpoint.Address} to try again another day :)");
                _metaClient.Close();

                await StartServerAsync();
                return;
            }

            _tcpLoop = Task.Run(() =>
            {
                while (_metaClient.Connected)
                {
                    if (_metaStream.DataAvailable)
                    {
                        byte[] buffer = new byte[Constants.MetaFrameLength];
                        _metaStream.Read(buffer, 0, Constants.MetaFrameLength);

                        string[] metapacket = Encoding.UTF8.GetString(buffer).Replace("\0", "").Split(Constants.ParameterSeparator);

                        switch ((MetaHeader)int.Parse(metapacket[0]))
                        {
                            case MetaHeader.Key:
                                Log($"Recieved key: {metapacket[1]}");
                                break;
                                /*
                                case MetaHeader.UDPReady:
                                    videoStream = new UdpClient(Constants.VideoStreamPort);
                                    IPEndPoint ip = metaStream.Client.RemoteEndPoint as IPEndPoint;
                                    videoStream.Connect(ip.Address, Constants.VideoStreamPort);
                                    break;*/
                        }
                    }
                }
                Log("Connection lost... or disconnected(tcp loop)");
                _tcpLoop.Dispose();
            });
        }

        private async Task StartServerAsync()
        {
            Log("Starting server...");
            _clientListener?.Stop();
            _clientListener = new TcpListener(_acceptedAddress, Constants.MetaStreamPort);
            _clientListener.Start();
            Log($"Server started {_acceptedAddress}:{Constants.MetaStreamPort}");


            _metaClient = await _clientListener.AcceptTcpClientAsync();
            _metaStream = _metaClient.GetStream();
            Log("Connection recieved...");
            await BeginHandshakeAsync();
        }

        private void BlockIPAddress(IPAddress ip)
        {
            Console.WriteLine("Blocking IP: " + ip.ToString());
        }

        private void WindowCapture_Resize(object sender, EventArgs e) => UpdateResolution();

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

        private void NotifyResolutionChange()
        {
            if (_metaClient?.Connected == false)
            {
                Log("Not connected...");
                return;
            }

            Networking.ResolutionChange(_metaStream, _videoResolution);
        }

        private void UpdateResolution()
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
        }

        private void Log(object stdout, [CallerLineNumber] int line=0)
        {
            string time = DateTime.Now.ToString("h:mm:ss:FFF");

            toolStripStatusLabelLatest.Text = "[" + time + "] " + stdout.ToString();
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
                    NotifyResolutionChange();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    _fullscreen = false;
                    this.Size = _lastResolution;
                    NotifyResolutionChange();
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