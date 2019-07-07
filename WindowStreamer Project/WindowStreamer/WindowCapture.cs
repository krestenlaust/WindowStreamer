using System;
using System.Diagnostics;
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

namespace Server
{
    public partial class WindowCapture : Form
    {
        public IntPtr TargetWindowHandle { get; private set; } = IntPtr.Zero;

        private UdpClient videoStream;
        private TcpClient metaStream = new TcpClient(AddressFamily.InterNetwork);
        private TcpListener metaStreamListener;
        private IPAddress acceptedAddress = IPAddress.Any;
        private List<IPAddress> blockedAddresses = new List<IPAddress>();
        private Size videoResolution;
        private bool fullscreen = false;
        private bool streamVideo = true;
        //private System.Timers.Timer framerateTimer;
        private double fps = 10;
        private Point captureAreaTopLeft;
        //private Size captureSize;
        private Cursor applicationSelectorCursor;
        //private bool applicationSelector = false;
        private Task tcpLoop = null;
        private Size lastResolution;
        private Size fullscreenSize = new Size
        {
            Width = 400,
            Height = 100
        };
        private Task streamingCycle = null;
        private long lastSentFrame;
        
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
            //videoStream = new UdpClient(Constants.VideoStreamPort);


            //framerateTimer = new System.Timers.Timer();
            //framerateTimer.Interval = 1000 / fps;
            //framerateTimer.Elapsed += new ElapsedEventHandler(OnFrameTick);
        }

        //private async void OnFrameTick(object source, ElapsedEventArgs e)
        private async Task NewFrame()
        {
            if (metaStream.Connected && streamVideo) //&& Settings.VideoStreaming)
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
                Bitmap bmp = new Bitmap(videoResolution.Width, videoResolution.Height);
                byte[] bytes = bmp.ToByteArray(System.Drawing.Imaging.ImageFormat.Bmp);
                await videoStream.SendAsync(bytes, bytes.Length);
                Log("Sent frame");

                //await VideoStream.SendAsync(imagePayload, imagePayload.Length);
            }
        }

        private async Task StreamCycle()
        {
            while (metaStream.Connected)
            {
                if (fps == 0)
                {
                    lastSentFrame = External.TimeStamp();
                    await NewFrame();
                }
                else
                {
                    if (External.TimeStamp() - lastSentFrame > 1000 / fps)
                    {
                        lastSentFrame = External.TimeStamp();
                        await NewFrame();
                    }
                }
            }
        }

        private async Task BeginHandshakeAsync()
        {
            IPEndPoint clientIPEndPoint = metaStream.Client.RemoteEndPoint as IPEndPoint;
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
                    metaStream.Close();

                    StartServerAsync();
                    return;

                case DialogResult.Ignore: //Ignore
                    metaStream.Close();
                    Log("Ignored connection");

                    StartServerAsync();
                    return;

                case DialogResult.Yes: //Accept
                    handshakeString = ((int)MetaHeader.ConnectionReply).ToString() + 
                        Constants.ParameterSeparator + '1' + 
                        Constants.ParameterSeparator + videoResolution.Width + Constants.SingleSeparator + videoResolution.Height + 
                        Constants.ParameterSeparator + Constants.VideoStreamPort;

                    videoStream = new UdpClient(Constants.VideoStreamPort);
                    break;
                case DialogResult.No: //Deny
                    handshakeString = ((int)MetaHeader.ConnectionReply).ToString() + 
                        Constants.ParameterSeparator + '0';
                    break;
                default:
                    return;
            }

            NetworkStream dataStream = metaStream.GetStream();

            byte[] bytes = Encoding.UTF8.GetBytes(handshakeString);
            External.PadArray(ref bytes, Constants.MetaFrameLength);

            await dataStream.WriteAsync(bytes, 0, bytes.Length);
            Log($"Handshake: [{handshakeString}]");
            
            if (handshakeString.Split(',')[1] == "0")
            {
                Log($"Told {clientIPEndPoint.Address} to try again another day :)");
                metaStream.Close();

                StartServerAsync();
                return;
            }


            tcpLoop = Task.Run(async () =>
            {
                NetworkStream newDataStream = metaStream.GetStream();
                while (metaStream.Connected)
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
            NetworkStream dataStream = metaStream.GetStream();
            byte[] buffer = new byte[Constants.MetaFrameLength];
            await dataStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

            string[] metapacket = Encoding.UTF8.GetString(buffer).Replace("\0", "").Split(Constants.ParameterSeparator);

            switch ((MetaHeader)int.Parse(metapacket[0]))
            {
                case MetaHeader.Key:
                    
                    break;
                /*
                case MetaHeader.UDPReady:
                    videoStream = new UdpClient(Constants.VideoStreamPort);
                    IPEndPoint ip = metaStream.Client.RemoteEndPoint as IPEndPoint;
                    videoStream.Connect(ip.Address, Constants.VideoStreamPort);
                    break;*/
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
            metaStreamListener = new TcpListener(acceptedAddress, Constants.MetaStreamPort);
            metaStreamListener.Start();
            Log($"Server started {acceptedAddress}:{Constants.MetaStreamPort}");

            try
            {
                metaStream = await metaStreamListener.AcceptTcpClientAsync();
                if (metaStream.Connected)
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
                    fullscreen = true;
                    lastResolution = this.Size;
                    this.Size = fullscreenSize;
                    UpdateResolution();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    fullscreen = false;
                    this.Size = lastResolution;
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
                acceptedAddress = IPAddress.Any;
            }
            else if (!IPAddress.TryParse(toolStripTextBoxAcceptableHost.Text, out acceptedAddress))
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
            if (fullscreen)
            {
                videoResolution.Height = Screen.FromControl(this).Bounds.Height;
                videoResolution.Width = Screen.FromControl(this).Bounds.Width;
                captureAreaTopLeft = Screen.FromControl(this).Bounds.Location;
            }
            else
            {
                videoResolution.Height = captureArea.Height;
                videoResolution.Width = captureArea.Width;
                captureAreaTopLeft = captureArea.Bounds.Location;
            }

            toolStripStatusLabelResolution.Text = videoResolution.Width.ToString() + "x" + videoResolution.Height.ToString();

            if (metaStream.Connected)
            {
                string Payload = ((int)MetaHeader.ResolutionUpdate).ToString() +
                    "," +
                    videoResolution.ToString();

                metaStream.GetStream().BeginWrite(Encoding.UTF8.GetBytes(Payload),
                    0,
                    Encoding.UTF8.GetByteCount(Payload),
                    new AsyncCallback(CallbackVoid),
                    null);
            }
        }

        private void Log(object stdout)
        {
            toolStripStatusLabelLatest.Text = stdout.ToString();
            System.Diagnostics.Debug.WriteLine("[Server] " + stdout);
        }

        private byte[] CompressImage(DirectBitmap img) { return Encoding.UTF8.GetBytes(img.Bits.ToString()); }

        private void StartLogWindow()
        {
            if (Other.Statics.logWindow != null)
            {
                
            }
            LogWindow lw = new LogWindow();
            lw.FormClosed += new FormClosedEventHandler(LogWindowClosed);
        }

        private void LogWindowClosed(object sender, FormClosedEventArgs e)
        {
            LogWindow lw = sender as LogWindow;
            Console.SetOut(lw.sw);
        }
    }

    public class WindowActions
    {
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        internal struct INPUT
        {
            public int Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
        }
        internal struct MOUSEINPUT
        {
            public int X;
            public int Y;
            public uint MouseData;
            public uint Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        public static void ClickOnPoint(IntPtr handle, Point point, int mouseButton = 0)
        {
            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Handle not attached");
                return;
            }

            Point cursorPosition = Cursor.Position;

            ClientToScreen(handle, ref point);

            Cursor.Position = new Point(point.X, point.Y);

            INPUT inputMouseDown = new INPUT();
            inputMouseDown.Type = 0; /// input type mouse
            inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

            INPUT inputMouseUp = new INPUT();
            inputMouseUp.Type = 0; /// input type mouse
            inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

            var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            /// return mouse 
            Cursor.Position = cursorPosition;
        }

        public static void InputKey(IntPtr handle, Keys key)
        {

        }
    }
}