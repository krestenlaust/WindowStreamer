using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Shared;
using Shared.Protocol;
using System.Timers;
using System.Threading.Tasks;

namespace Server
{
    public partial class WindowCapture : Form
    {
        private UdpClient VideoStream;
        private TcpClient MetaStream = new TcpClient(AddressFamily.InterNetwork);
        private TcpListener MetaStreamListener;
        private IPAddress AcceptedAddress = IPAddress.Any;
        private Size VideoResolution;
        private bool Fullscreen = false;
        private System.Timers.Timer framerateTimer;
        private double fps = 10;
        private Point CaptureAreaTopLeft;
        private Size CaptureSize;
        private Cursor applicationSelectorCursor;
        private bool ApplicationSelector = false;
        private Task tcpLoop = null;

        private Size LastResolution;
        private Size FullscreenSize = new Size
        {
            Width = 400,
            Height = 100
        };

        public WindowCapture()
        {
            InitializeComponent();
        }

        private void WindowCapture_Load(object sender, EventArgs e)
        {
            //Stream cursorStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("applicationDropperCursor");
            //applicationSelectorCursor = new Cursor(cursorStream);
            applicationSelectorCursor = Cursors.Hand;

            TransparencyKey = Color.Orange;
            captureArea.BackColor = Color.Orange;
            UpdateResolution();

            toolStripTextBoxTargetPort.Text = Constants.MetaStreamPort.ToString();

            //MetaStream = new TcpClient();
            VideoStream = new UdpClient(Constants.VideoStreamPort);

            framerateTimer = new System.Timers.Timer();
            framerateTimer.Interval = 1000 / fps;
            framerateTimer.Elapsed += new ElapsedEventHandler(OnFrameTick);
        }

        private async void OnFrameTick(object source, ElapsedEventArgs e)
        {
            if (MetaStream.Connected) //&& Settings.VideoStreaming)
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
                Bitmap bmp = new Bitmap(10, 10);
                byte[] bytes = bmp.ToByteArray(System.Drawing.Imaging.ImageFormat.Bmp);
                await VideoStream.SendAsync(bytes, bytes.Length);

                //await VideoStream.SendAsync(imagePayload, imagePayload.Length);
            }
        }

        private async Task BeginHandshakeAsync()
        {
            IPEndPoint clientIPEndPoint = MetaStream.Client.RemoteEndPoint as IPEndPoint;
            DialogResult diagres = DialogResult.Ignore;

            string handshakeString;

            await Task.Run(() =>
            {
                Log("Inbound connection, awaiting prompt return...");
                ConnectionPrompt prompt = new ConnectionPrompt();
                prompt.IPAddress = clientIPEndPoint.Address.ToString();
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.TopMost = true;
                diagres = prompt.ShowDialog();
            });
            Log("Prompt returned...");

            switch (diagres)
            {
                case DialogResult.Abort: //Block
                    BlockIPAddress(clientIPEndPoint.Address);
                    Log("Blocked the following IP Address: " + clientIPEndPoint.Address);

                    MetaStream.Close();
                    return;

                case DialogResult.Ignore: //Ignore
                    MetaStream.Close();
                    Log("Ignored connection");
                    return;

                case DialogResult.Yes: //Accept
                    handshakeString = MetaHeader.ConnectionReply.ToString() + 
                        ',' + '1' +
                        ',' + CaptureSize.Width + '.' + CaptureSize.Height;
                    break;
                case DialogResult.No: //Deny
                    handshakeString = MetaHeader.ConnectionReply.ToString() +
                        ',' + '0';
                    break;
                default:
                    return;
            }

            NetworkStream dataStream = MetaStream.GetStream();

            byte[] bytes = Encoding.UTF8.GetBytes(handshakeString);
            External.PadArray(ref bytes, Constants.MetaFrameLength);

            await dataStream.WriteAsync(bytes, 0, 0);
            Log("Answered request, awaiting client answer...");

            if (handshakeString.Split(',')[1] == "0")
            {
                Log($"Told {clientIPEndPoint.Address} to try again another day :)");
                MetaStream.Close();
                return;
            }


            tcpLoop = Task.Run(async () =>
            {
                NetworkStream newDataStream = MetaStream.GetStream();
                while (MetaStream.Connected)
                {
                    if (External.NetworkStreamLength(ref newDataStream) > 0)
                    {
                        await MetaPacketReceivedAsync();
                    }
                }
                Log("Connection lost... or disconnected");
                tcpLoop.Dispose();
            });

            Log("Handshake finished");
        }

        private async Task MetaPacketReceivedAsync()
        {
            NetworkStream dataStream = MetaStream.GetStream();
            byte[] buffer = new byte[Constants.MetaFrameLength];
            await dataStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

            string[] metapacket = Encoding.UTF8.GetString(buffer).Split(',');

            switch ((MetaHeader)int.Parse(metapacket[0]))
            {
                case MetaHeader.UDPReady:
                    VideoStream = new UdpClient(Constants.VideoStreamPort);
                    IPEndPoint ip = MetaStream.Client.RemoteEndPoint as IPEndPoint;
                    VideoStream.Connect(ip.Address, Constants.VideoStreamPort);
                    break;
            }
        }

        /*
        private void MetaFrameListener()
        {
            NetworkStream dataStream = MetaStream.GetStream();
            byte messageType = (byte)dataStream.ReadByte();
            byte[] lengthBuffer = new byte[sizeof(int)];
            int recv = dataStream.Read(lengthBuffer, 0, lengthBuffer.Length);
            if (recv == sizeof(int))
            {
                int messageLen = BitConverter.ToInt32(lengthBuffer, 0);
                byte[] messageBuffer = new byte[messageLen];
                recv = dataStream.Read(messageBuffer, 0, messageBuffer.Length);
                if (recv == messageLen)
                {
                    // messageBuffer contains your whole message ...
                }
            }
        }*/

        private async Task StartServerAsync()
        {
            Log("Starting server...");
            MetaStreamListener = new TcpListener(AcceptedAddress, Constants.MetaStreamPort);
            MetaStreamListener.Start();
            Log($"Server started {AcceptedAddress}:{Constants.MetaStreamPort}");

            try
            {
                MetaStream = await MetaStreamListener.AcceptTcpClientAsync();
                if (MetaStream.Connected)
                {
                    Log("Connection recieved...");
                    await BeginHandshakeAsync();
                }
            }
            catch (SocketException) {}
        }

        private void BlockIPAddress(IPAddress ip)
        {
            Console.WriteLine("Blocking ip: " + ip.ToString());
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    Fullscreen = true;
                    LastResolution = this.Size;
                    this.Size = FullscreenSize;
                    UpdateResolution();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    Fullscreen = false;
                    this.Size = LastResolution;
                    UpdateResolution();
                }
            }
            base.WndProc(ref m);
        }

        #region Application Droplet function
        //https://social.msdn.microsoft.com/Forums/en-US/bfc75b57-df16-48c6-92af-ea0a34f540ae/how-to-get-the-handle-of-a-window-that-i-click?forum=csharplanguage
        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        static IntPtr hHook = IntPtr.Zero;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);


        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                //  The application runs to here when you click on the window whose handle you  want to get                
                POINT cusorPoint;
                bool ret = GetCursorPos(out cusorPoint);
                // cusorPoint contains your cusor’s position when you click on the window


                // Then use cusorPoint to get the handle of the window you clicked

                IntPtr winHandle = WindowFromPoint(cusorPoint);

                MessageBox.Show(winHandle.ToString());

                // Because the hook may occupy much memory, so remember to uninstall the hook after
                // you finish your work, and that is what the following code does.
                UnhookWindowsHookEx(hHook);
                hHook = IntPtr.Zero;

                #region Comment block, ain't mine
                // Here I do not use the GetActiveWindow(). Let's call the window you clicked "DesWindow" and explain my reason.
                // I think the hook intercepts the mouse click message before the mouse click message delivered to the DesWindow's 
                // message queue. The application came to this function before the DesWindow became the active window, so the handle 
                // abtained from calling GetActiveWindow() here is not the DesWindow's handle, I did some tests, and What I got is always 
                // the Form's handle, but not the DesWindow's handle. You can do some test too.

                //IntPtr handle = GetActiveWindow();
                #endregion
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201, WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200, WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204, WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x; public int y;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT Point);

        public void AddGlobalClickHook()
        {
            if (IntPtr.Zero == hHook)
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    hHook = SetWindowsHookEx(WH_MOUSE_LL,
                        _proc,
                        GetModuleHandle(curModule.ModuleName),
                        0);
                }
            }
        }
        #endregion


        private void CallbackVoid(IAsyncResult ar) { }

        private void WindowCapture_Resize(object sender, EventArgs e) => UpdateResolution();

        private void toolStripButtonFocusOnWindow_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            if (toolStripTextBoxAcceptableHost.Text == "")
            {
                AcceptedAddress = IPAddress.Any;
            } else if (!IPAddress.TryParse(toolStripTextBoxAcceptableHost.Text, out AcceptedAddress))
            {
                MessageBox.Show("IP not valid", "Error");
                return;
            }

            Console.WriteLine("Starting server");

            StartServerAsync();

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


        private void UpdateResolution()
        {
            if (Fullscreen)
            {
                VideoResolution.Height = Screen.FromControl(this).Bounds.Height;
                VideoResolution.Width = Screen.FromControl(this).Bounds.Width;
                CaptureAreaTopLeft = Screen.FromControl(this).Bounds.Location;
            }
            else
            {
                VideoResolution.Height = captureArea.Height;
                VideoResolution.Width = captureArea.Width;
                CaptureAreaTopLeft = captureArea.Bounds.Location;
            }

            toolStripStatusLabelResolution.Text = VideoResolution.Width.ToString() + "x" + VideoResolution.Height.ToString();

            if (MetaStream.Connected)
            {
                string Payload = ((int)MetaHeader.ResolutionUpdate).ToString() +
                    "," +
                    VideoResolution.ToString();

                MetaStream.GetStream().BeginWrite(Encoding.UTF8.GetBytes(Payload),
                    0,
                    Encoding.UTF8.GetByteCount(Payload),
                    new AsyncCallback(CallbackVoid),
                    null);
            }
        }

        private void Log(object stdout) { toolStripStatusLabelLatest.Text = stdout.ToString(); }

        private byte[] CompressImage(DirectBitmap img) { return Encoding.UTF8.GetBytes(img.Bits.ToString()); }
    }
}