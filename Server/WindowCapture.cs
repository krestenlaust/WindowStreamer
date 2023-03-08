using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelSink;
using Serilog;

namespace Server
{
    public partial class WindowCapture : Form
    {
        static readonly int DefaultMetastreamPort = 10063;
        static readonly Color transparencyKeyColor = Color.Orange;

        bool fullscreen;
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

        WindowServer server;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCapture"/> class.
        /// </summary>
        public WindowCapture()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File("log-server.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.ToolStripLabel(toolStripStatusLabelLatest)
                .CreateLogger();
        }

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

        async void WindowCapture_LoadAsync(object sender, EventArgs e)
        {
            TransparencyKey = transparencyKeyColor;
            captureArea.BackColor = transparencyKeyColor;
            UpdateResolutionVariables();
            toolStripTextBoxTargetPort.Text = DefaultMetastreamPort.ToString();

            // Update UI
            ToggleActionStartStopButtons(false);

            // Handle command-line arguments
            await HandleCommandlineArgumentsAsync(Environment.GetCommandLineArgs());
        }

        async Task HandleCommandlineArgumentsAsync(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (!args[i].StartsWith("-"))
                {
                    continue;
                }

                switch (args[i])
                {
                    case "--listen":
                    case "-l":
                        if (args.Length == i + 1)
                        {
                            // No parameter given.
                            continue;
                        }

                        string parameter = args[i + 1];

                        var splittedParameter = parameter.Split(':');

                        if (!IPAddress.TryParse(splittedParameter[0], out IPAddress address))
                        {
                            // Invalid parameter
                            continue;
                        }

                        if (splittedParameter.Length == 1 || !int.TryParse(splittedParameter[1], out int targetPort))
                        {
                            targetPort = DefaultMetastreamPort;
                        }

                        await StartServerAsync(address, targetPort);
                        break;
                    default:
                        break;
                }
            }
        }

        Bitmap ObtainImage()
        {
            Size size = new Size(videoResolution.Width, videoResolution.Height);
            return GetScreenPicture(captureArea.Location.X + Location.X, captureArea.Location.Y + Location.Y + toolStripHeader.Height, size);
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

        void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }

        async void toolStripButtonActionStart_ClickAsync(object sender, EventArgs e)
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

            if (!int.TryParse(toolStripTextBoxTargetPort.Text, out int targetPort))
            {
                targetPort = DefaultMetastreamPort;
            }

            await StartServerAsync(acceptedAddress, targetPort);
        }

        void toolStripButtonActionStop_Click(object sender, EventArgs e)
        {
            server?.Dispose();
            server = null;
            ToggleActionStartStopButtons(false);

            Log.Information("Stopped server");
        }

        void ToggleActionStartStopButtons(bool serverRunning)
        {
            toolStripButtonActionStart.Visible = !serverRunning;
            toolStripButtonActionStop.Visible = serverRunning;
        }

        async Task StartServerAsync(IPAddress bindAddress, int port = 0)
        {
            if (port == 0)
            {
                port = DefaultMetastreamPort;
            }

            if (server is not null)
            {
                server.ConnectionClosed -= WindowServer_ConnectionClosed;
                server.Dispose();
            }

            ToggleActionStartStopButtons(true);

            server = new WindowServer(bindAddress, port, videoResolution, ObtainImage, HandleConnectionReply);
            server.ConnectionClosed += WindowServer_ConnectionClosed;

            try
            {
                await server.StartServerAsync().ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                Log.Information($"Couldn't start server: {ex}");
                ToggleActionStartStopButtons(false);
            }
        }

        private void WindowServer_ConnectionClosed()
        {
            Log.Information("Client disconnected");
            ToggleActionStartStopButtons(false);
        }

        void UpdateResolutionVariables()
        {
            if (fullscreen)
            {
                videoResolution.Height = Screen.FromControl(this).Bounds.Height;
                videoResolution.Width = Screen.FromControl(this).Bounds.Width;
                //captureAreaTopLeft = Screen.FromControl(this).Bounds.Location;
            }
            else
            {
                videoResolution.Height = captureArea.Height;
                videoResolution.Width = captureArea.Width;
                //captureAreaTopLeft = captureArea.Bounds.Location;
            }

            toolStripStatusLabelResolution.Text = $"{videoResolution.Width}x{videoResolution.Height}";
            server?.UpdateResolution(videoResolution);
        }

        void LogWindowClosed(object sender, FormClosedEventArgs e)
        {
            LogWindow lw = sender as LogWindow;
            Console.SetOut(lw.sw);
        }

        static Bitmap GetScreenPicture(int x, int y, Size size)
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

            var rect = new Rectangle(x, y, size.Width, size.Height);
            var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

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