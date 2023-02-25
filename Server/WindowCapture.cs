using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using LabelSink;
using Serilog;
using Shared;

namespace Server
{
    public partial class WindowCapture : Form
    {
        readonly Color transparencyKeyColor = Color.Orange;

        bool fullscreen = false;
        Size videoResolution;

        /// <summary>
        /// Not sure what this field keeps track of.
        /// </summary>
        Size lastResolution;
        Size fullscreenSize = new Size
        {
            Width = 400,
            Height = 100,
        };

        Point captureAreaTopLeft;
        Cursor applicationSelectorCursor = Cursors.Hand;

        WindowServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCapture"/> class.
        /// </summary>
        public WindowCapture()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-server.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.ToolStripLabel(toolStripStatusLabelLatest)
                .CreateLogger();
        }

        public IntPtr TargetWindowHandle { get; private set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    fullscreen = true;
                    lastResolution = Size;
                    Size = fullscreenSize;
                    UpdateResolutionVariables();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    fullscreen = false;
                    Size = lastResolution;
                    UpdateResolutionVariables();
                }
            }

            base.WndProc(ref m);
        }

        void WindowCapture_Load(object sender, EventArgs e)
        {
            TransparencyKey = transparencyKeyColor;
            captureArea.BackColor = transparencyKeyColor;

            UpdateResolutionVariables();

            toolStripTextBoxTargetPort.Text = DefaultValues.MetaStreamPort.ToString();
        }

        (Bitmap, Size) ObtainImage()
        {
            return (
                GetScreenPicture(captureArea.Location.X + Location.X, captureArea.Location.Y + Location.Y, videoResolution.Width, videoResolution.Height),
                captureArea.Size);
        }

        WindowServer.ConnectionReply HandleConnectionReply(IPAddress ipAddress)
        {
            var prompt = new ConnectionPrompt(ipAddress.ToString())
            {
                StartPosition = FormStartPosition.CenterParent,
                TopMost = true,
            };

            switch (prompt.ShowDialog())
            {
                case DialogResult.Abort: // Block
                    Log.Information("Blocked the following IP Address: " + ipAddress);

                    BlockIPAddress(ipAddress);
                    return WindowServer.ConnectionReply.Close;

                case DialogResult.Ignore: // Ignore
                    Log.Information("Ignoring connection");
                    return WindowServer.ConnectionReply.Close;

                case DialogResult.Yes: // Accept
                    Log.Information("Accepting connection");
                    return WindowServer.ConnectionReply.Accept;

                case DialogResult.No: // Deny
                    Log.Information($"Told {ipAddress} to try again another day :)");
                    return WindowServer.ConnectionReply.Deny;
                default:
                    break;
            }

            return 0;
        }

        void BlockIPAddress(IPAddress ip)
        {
            // TODO: (#15) Implement blocking IPs
            Console.WriteLine("Blocking IP: " + ip.ToString());
        }

        void WindowCapture_Resize(object sender, EventArgs e) => UpdateResolutionVariables();

        void toolStripButtonFocusOnWindow_Click(object sender, EventArgs e)
        {
        }

        void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }

        async void toolStripButtonConnect_ClickAsync(object sender, EventArgs e)
        {
            IPAddress acceptedAddress;
            if (toolStripTextBoxAcceptableHost.Text == string.Empty)
            {
                acceptedAddress = IPAddress.Any;
            }
            else if (!IPAddress.TryParse(toolStripTextBoxAcceptableHost.Text, out acceptedAddress))
            {
                MessageBox.Show("IP not valid", "Error");
                return;
            }

            server = new WindowServer(acceptedAddress, videoResolution, ObtainImage, HandleConnectionReply);
            await server.StartServerAsync();
        }

        void toolStripButtonApplicationPicker_MouseHover(object sender, EventArgs e)
        {
        }

        void toolStripButtonApplicationPicker_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void toolStripButtonApplicationSelector_Click(object sender, EventArgs e)
        {
            toolStripButtonApplicationSelector.ToolTipText = "Click here";
        }

        void UpdateResolutionVariables()
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

            server?.UpdateResolution(videoResolution);
        }

        /*
        void Log(object message) => Log(message, 0);

        void Log(object stdout, [CallerLineNumber] int line = 0)
        {
            string time = DateTime.Now.ToString("mm:ss:ffff");

            if (IsHandleCreated)
            {
                Invoke((MethodInvoker)delegate
                {
                    toolStripStatusLabelLatest.Text = "[" + time + "] " + stdout;
                });
            }

            Debug.WriteLine($"[{time}][{line}][Server] {stdout}");
        }*/

        /*
        private void StartLogWindow()
        {
            if (Other.Statics.LogWindow != null)
            {
            }
            LogWindow lw = new LogWindow();
            lw.FormClosed += new FormClosedEventHandler(LogWindowClosed);
        }*/

        void LogWindowClosed(object sender, FormClosedEventArgs e)
        {
            LogWindow lw = sender as LogWindow;
            Console.SetOut(lw.sw);
        }

        void toolStripButtonDebug1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        static Bitmap GetScreenPicture(int x, int y, int width, int height)
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

            var rect = new Rectangle(x, y, width, height);
            var bmp = new Bitmap(rect.Width, rect.Height, DefaultValues.ImageFormat);

            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            return bmp;
        }

        private void WindowCapture_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Information("Application closing...");
            Log.CloseAndFlush();
        }
    }
}