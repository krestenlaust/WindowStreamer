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

namespace Server
{
    public partial class WindowCapture : Form
    {
        public IntPtr TargetWindowHandle { get; private set; } = IntPtr.Zero;

        //private UdpClient videoStream;
        private IPEndPoint ClientEndpoint;
        private TcpClient MetaClient = new TcpClient(AddressFamily.InterNetwork);
        private NetworkStream MetaStream;
        private TcpListener ClientListener;
        private IPAddress acceptedAddress = IPAddress.Any;
        private List<IPAddress> blockedAddresses = new List<IPAddress>();
        private Size videoResolution;
        private bool fullscreen = false;
        private bool streamVideo = true;
        private double fps = 10;
        private Point captureAreaTopLeft;
        private Cursor applicationSelectorCursor;
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
            applicationSelectorCursor = Cursors.Hand;

            TransparencyKey = Color.Orange;
            captureArea.BackColor = Color.Orange;
            UpdateResolution();

            toolStripTextBoxTargetPort.Text = Constants.MetaStreamPort.ToString();
        }
        
        private async Task NewFrame()
        {
            if (MetaClient.Connected && streamVideo) //&& Settings.VideoStreaming)
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
                //await videoStream.SendAsync(bytes, bytes.Length);
                Log("Sending frame");
                using (UdpClient sender = new UdpClient(0))
                {
                    await sender.SendAsync(bytes, bytes.Length, ClientEndpoint.Address.ToString(), Constants.VideoStreamPort);
                }
                Log("Sent frame");

                //await VideoStream.SendAsync(imagePayload, imagePayload.Length);
            }
        }

        private async Task StreamCycle()
        {
            if (MetaClient.Connected)
            {
                if (fps == 0)
                {
                    lastSentFrame = External.TimeStamp();
                    await NewFrame();
                }
            }
            while (MetaClient.Connected)
            {
                if (External.TimeStamp() - lastSentFrame > 1000 / fps)
                {
                    lastSentFrame = External.TimeStamp();
                    await NewFrame();
                    Log("Fixed Framerate tick");
                }
            }
            Log("Metastream not connected");
        }

        private async Task BeginHandshakeAsync()
        {
            ClientEndpoint = MetaClient.Client.RemoteEndPoint as IPEndPoint;

            DialogResult diaglogResult = DialogResult.Ignore;


            await Task.Run(() =>
            {
                Log("Inbound connection, awaiting prompt return...");
                ConnectionPrompt prompt = new ConnectionPrompt();
                prompt.IPAddress = ClientEndpoint.Address.ToString();
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.TopMost = true;
                diaglogResult = prompt.ShowDialog();
            });
            Log("Prompt returned...");

            string handshakeString;

            switch (diaglogResult)
            {
                case DialogResult.Abort: //Block
                    BlockIPAddress(ClientEndpoint.Address);
                    Log("Blocked the following IP Address: " + ClientEndpoint.Address);
                    MetaClient.Close();

                    await StartServerAsync();
                    return;

                case DialogResult.Ignore: //Ignore
                    MetaClient.Close();
                    Log("Ignored connection");

                    await StartServerAsync();
                    return;

                case DialogResult.Yes: //Accept
                    handshakeString = ((int)MetaHeader.ConnectionReply).ToString() + 
                        Constants.ParameterSeparator + '1' + 
                        Constants.ParameterSeparator + videoResolution.Width + Constants.SingleSeparator + videoResolution.Height + 
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

            await MetaStream.WriteAsync(bytes, 0, bytes.Length);
            Log($"Handshake: [{handshakeString}]");
            
            if (handshakeString.Split(',')[1] == "0")
            {
                Log($"Told {ClientEndpoint.Address} to try again another day :)");
                MetaClient.Close();

                await StartServerAsync();
                return;
            }

            tcpLoop = Task.Run(() =>
            {
                while (MetaClient.Connected)
                {
                    if (MetaStream.DataAvailable)
                    {
                        byte[] buffer = new byte[Constants.MetaFrameLength];
                        MetaStream.Read(buffer, 0, Constants.MetaFrameLength);

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
                tcpLoop.Dispose();
            });
        }

        private async Task StartServerAsync()
        {
            Log("Starting server...");
            ClientListener?.Stop();
            ClientListener = new TcpListener(acceptedAddress, Constants.MetaStreamPort);
            ClientListener.Start();
            Log($"Server started {acceptedAddress}:{Constants.MetaStreamPort}");


            MetaClient = await ClientListener.AcceptTcpClientAsync();
            MetaStream = MetaClient.GetStream();
            Log("Connection recieved...");
            await BeginHandshakeAsync();
        }

        private void BlockIPAddress(IPAddress ip)
        {
            Console.WriteLine("Blocking IP: " + ip.ToString());
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

        private async void toolStripButtonConnect_ClickAsync(object sender, EventArgs e)
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


        private void UpdateResolution()
        {
            if (fullscreen)
            {
                videoResolution.Height = Screen.FromControl(this).Bounds.Height;
                videoResolution.Width = Screen.FromControl(this).Bounds.Width;
                captureAreaTopLeft = Screen.FromControl(this).Bounds.Location;
            } else
            {
                videoResolution.Height = captureArea.Height;
                videoResolution.Width = captureArea.Width;
                captureAreaTopLeft = captureArea.Bounds.Location;
            }

            toolStripStatusLabelResolution.Text = videoResolution.Width.ToString() + "x" + videoResolution.Height.ToString();

            if (MetaClient.Connected)
            {
                string Payload = ((int)MetaHeader.ResolutionUpdate).ToString() +
                    "," +
                    videoResolution.ToString();

                MetaClient.GetStream().BeginWrite(Encoding.UTF8.GetBytes(Payload),
                    0,
                    Encoding.UTF8.GetByteCount(Payload),
                    new AsyncCallback(CallbackVoid),
                    null);
            }
        }

        private void Log(object stdout)
        {
            string time = DateTime.Now.ToString("h:mm:ss:FFF");

            toolStripStatusLabelLatest.Text = "[" + time + "] " + stdout.ToString();
            Debug.WriteLine("["+time+"][Server] " + stdout);
        }

        private byte[] CompressImage(DirectBitmap img)
        {
            return Encoding.UTF8.GetBytes(img.Bits.ToString());
        }

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

        private void toolStripButtonDebug1_Click(object sender, EventArgs e)
        {

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
    }

    public static class WindowActions
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