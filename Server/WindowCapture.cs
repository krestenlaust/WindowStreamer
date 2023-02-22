using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Shared;

namespace Server
{
    public partial class WindowCapture : Form
    {
        private bool fullscreen = false;
        private Size videoResolution;

        /// <summary>
        /// Not sure what this field keeps track of.
        /// </summary>
        private Size lastResolution;
        private Size fullscreenSize = new Size
        {
            Width = 400,
            Height = 100,
        };

        private Point captureAreaTopLeft;
        private Cursor applicationSelectorCursor;

        private WindowServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCapture"/> class.
        /// </summary>
        public WindowCapture()
        {
            InitializeComponent();
        }

        public IntPtr TargetWindowHandle { get; private set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0112) // WM_SYSCOMMAND
            {
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    fullscreen = true;
                    lastResolution = this.Size;
                    this.Size = fullscreenSize;
                    UpdateResolutionVariables();
                }
                else if (m.WParam == new IntPtr(0xF120))
                {
                    fullscreen = false;
                    this.Size = lastResolution;
                    UpdateResolutionVariables();
                }
            }

            base.WndProc(ref m);
        }

        private void WindowCapture_Load(object sender, EventArgs e)
        {
            applicationSelectorCursor = Cursors.Hand;

            TransparencyKey = Color.Orange;
            captureArea.BackColor = Color.Orange;
            UpdateResolutionVariables();

            toolStripTextBoxTargetPort.Text = DefaultValues.MetaStreamPort.ToString();
        }

        private (Bitmap, Size) ObtainImage()
        {
            return (
                GetScreenPicture(captureArea.Location.X + Location.X, captureArea.Location.Y + Location.Y, videoResolution.Width, videoResolution.Height),
                captureArea.Size);
        }

        private WindowServer.ConnectionReply HandleConnectionReply(IPAddress ipAddress)
        {
            ConnectionPrompt prompt = new ConnectionPrompt(ipAddress.ToString())
            {
                StartPosition = FormStartPosition.CenterParent,
                TopMost = true,
            };

            switch (prompt.ShowDialog())
            {
                case DialogResult.Abort: // Block
                    Log("Blocked the following IP Address: " + ipAddress);

                    BlockIPAddress(ipAddress);
                    return WindowServer.ConnectionReply.Close;

                case DialogResult.Ignore: // Ignore
                    Log("Ignoring connection");
                    return WindowServer.ConnectionReply.Close;

                case DialogResult.Yes: // Accept
                    Log("Accepting connection");
                    return WindowServer.ConnectionReply.Accept;

                case DialogResult.No: // Deny
                    Log($"Told {ipAddress} to try again another day :)");
                    return WindowServer.ConnectionReply.Deny;
                default:
                    break;
            }

            return 0;
        }

        private void BlockIPAddress(IPAddress ip)
        {
            // TODO: Actually block IP
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

            server = new WindowServer(acceptedAddress, videoResolution, ObtainImage, HandleConnectionReply, Log);
            await server.StartServerAsync();
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

        private void UpdateResolutionVariables()
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

        private void Log(object message) => Log(message, 0);

        private void Log(object stdout, [CallerLineNumber] int line = 0)
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
            if (Other.Statics.LogWindow != null)
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

        private static Bitmap GetScreenPicture(int x, int y, int width, int height)
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
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            return bmp;
        }
    }
}