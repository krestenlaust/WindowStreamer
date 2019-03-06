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
using System.IO.Compression;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Drawing.Imaging;

public enum Compression
{
    None, Decent, Heavy,
}

namespace WindowStreamer
{
    public partial class WindowCapture : Form
    {
        public UdpClient VideoStream;
        public TcpClient MetaStream = new TcpClient(AddressFamily.InterNetwork);
        public TcpListener MetaStreamListener;

        private IPAddress AcceptedAddress = IPAddress.Any;
        private Size VideoResolution;

        public bool Fullscreen = false;
        
        private System.Timers.Timer framerateTimer;
        private double fps = 10;

        private Point CaptureAreaTopLeft;
        private Size CaptureSize;

        private Cursor applicationSelectorCursor;

        //True when selecting application
        private bool ApplicationSelector = false;

        public WindowCapture()
        {
            InitializeComponent();
        }

        private void WindowCapture_Load(object sender, EventArgs e)
        {
            //Set preferences
            //Settings.SetDefault();


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
                /*
                DirectBitmap bmp = new DirectBitmap(CaptureSize.Width, CaptureSize.Height);
                Graphics g = Graphics.FromImage(bmp.Bitmap);

                g.CopyFromScreen(CaptureAreaTopLeft, CaptureAreaTopLeft, CaptureSize);
                g.Dispose();*/
                Bitmap bmp = new Bitmap(CaptureSize.Width, CaptureSize.Height);
                using (Graphics gfx = Graphics.FromImage(bmp))
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0)))
                {
                    gfx.FillRectangle(brush, 0, 0, CaptureSize.Width, CaptureSize.Height);
                }

                //byte[] imagePayload = CompressImage(bmp);
                LockBitmap lbmp = new LockBitmap(bmp);
                byte[] imagePayload = lbmp.Pixels;

                await VideoStream.SendAsync(imagePayload, imagePayload.Length);
            }
        }

        private byte[] CompressImage(DirectBitmap img)
        {
            //byte[] imagebytes = Encoding.UTF8.GetBytes(img.Bits.ToString());

            return Encoding.UTF8.GetBytes(img.Bits.ToString());
        }

        /*
        private async Task ListenForUdp()
        {
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, Constants.VideoStreamPort);
                var data = VideoStream.Receive(ref remoteEP);
                OnFrameTick(null, null);
                await Task.Delay(10);
            }
        }*/

        private void BeginHandshake()
        {
            NetworkStream dataStream = MetaStream.GetStream();
            //TODO: Write the rest of this handshake...

            framerateTimer.Start();
        }

        private async Task StartServerAsync()
        {
            MetaStreamListener = new TcpListener(AcceptedAddress, Constants.MetaStreamPort);
            MetaStreamListener.Start();
            try
            {
                MetaStream = await MetaStreamListener.AcceptTcpClientAsync();
                if (MetaStream.Connected)
                {
                    BeginHandshake();
                }

                //await MetaStream.ConnectAsync(AcceptedAddress, Constants.MetaStreamPort);

            }
            catch (SocketException e)
            {
            }


        }

        private void MetaReceived(IAsyncResult res)
        {
            
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    Fullscreen = true;
                    UpdateResolution();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    Fullscreen = false;
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


                // Here I do not use the GetActiveWindow(). Let's call the window you clicked "DesWindow" and explain my reason.
                // I think the hook intercepts the mouse click message before the mouse click message delivered to the DesWindow's 
                // message queue. The application came to this function before the DesWindow became the active window, so the handle 
                // abtained from calling GetActiveWindow() here is not the DesWindow's handle, I did some tests, and What I got is always 
                // the Form's handle, but not the DesWindow's handle. You can do some test too.

                //IntPtr handle = GetActiveWindow();
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
            }else if(!IPAddress.TryParse(toolStripTextBoxAcceptableHost.Text, out AcceptedAddress))
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
                Shared.Protocol.MetaHeader header = Shared.Protocol.MetaHeader.ResolutionUpdate;
                string Payload = ((int)header).ToString() + "," + VideoResolution.ToString();
                MetaStream.GetStream().BeginWrite(Encoding.UTF8.GetBytes(Payload), 0, Encoding.UTF8.GetByteCount(Payload), new AsyncCallback(CallbackVoid), null);
            }

        }
    }
}


public class LockBitmap
{
    Bitmap source = null;
    IntPtr Iptr = IntPtr.Zero;
    BitmapData bitmapData = null;

    public byte[] Pixels { get; set; }
    public int Depth { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public LockBitmap(Bitmap source)
    {
        this.source = source;
    }

    /// <summary>
    /// Lock bitmap data
    /// </summary>
    public void LockBits()
    {
        try
        {
            // Get width and height of bitmap
            Width = source.Width;
            Height = source.Height;

            // get total locked pixels count
            int PixelCount = Width * Height;

            // Create rectangle to lock
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // get source bitmap pixel format size
            Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

            // Check if bpp (Bits Per Pixel) is 8, 24, or 32
            if (Depth != 8 && Depth != 24 && Depth != 32)
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }

            // Lock bitmap and return bitmap data
            bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                         source.PixelFormat);

            // create byte array to copy pixel values
            int step = Depth / 8;
            Pixels = new byte[PixelCount * step];
            Iptr = bitmapData.Scan0;

            // Copy data from pointer to array
            Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Unlock bitmap data
    /// </summary>
    public void UnlockBits()
    {
        try
        {
            // Copy data from byte array to pointer
            Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

            // Unlock bitmap data
            source.UnlockBits(bitmapData);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Get the color of the specified pixel
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Color GetPixel(int x, int y)
    {
        Color clr = Color.Empty;

        // Get color components count
        int cCount = Depth / 8;

        // Get start index of the specified pixel
        int i = ((y * Width) + x) * cCount;

        if (i > Pixels.Length - cCount)
            throw new IndexOutOfRangeException();

        if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
        {
            byte b = Pixels[i];
            byte g = Pixels[i + 1];
            byte r = Pixels[i + 2];
            byte a = Pixels[i + 3]; // a
            clr = Color.FromArgb(a, r, g, b);
        }
        if (Depth == 24) // For 24 bpp get Red, Green and Blue
        {
            byte b = Pixels[i];
            byte g = Pixels[i + 1];
            byte r = Pixels[i + 2];
            clr = Color.FromArgb(r, g, b);
        }
        if (Depth == 8)
        // For 8 bpp get color value (Red, Green and Blue values are the same)
        {
            byte c = Pixels[i];
            clr = Color.FromArgb(c, c, c);
        }
        return clr;
    }

    /// <summary>
    /// Set the color of the specified pixel
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    public void SetPixel(int x, int y, Color color)
    {
        // Get color components count
        int cCount = Depth / 8;

        // Get start index of the specified pixel
        int i = ((y * Width) + x) * cCount;

        if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
        {
            Pixels[i] = color.B;
            Pixels[i + 1] = color.G;
            Pixels[i + 2] = color.R;
            Pixels[i + 3] = color.A;
        }
        if (Depth == 24) // For 24 bpp set Red, Green and Blue
        {
            Pixels[i] = color.B;
            Pixels[i + 1] = color.G;
            Pixels[i + 2] = color.R;
        }
        if (Depth == 8)
        // For 8 bpp set color value (Red, Green and Blue values are the same)
        {
            Pixels[i] = color.B;
        }
    }
}