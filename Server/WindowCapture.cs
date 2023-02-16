using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shared;
using Protocol;

namespace Server
{
    public partial class WindowCapture : Form
    {
        public IntPtr TargetWindowHandle { get; private set; } = IntPtr.Zero;

        //private UdpClient videoStream;
        private IPEndPoint clientEndpoint;
        private UdpClient videoClient;
        private TcpClient metaClient = new TcpClient(AddressFamily.InterNetwork);
        private NetworkStream metaStream;
        private TcpListener clientListener;
        private IPAddress acceptedAddress = IPAddress.Any;
        private Size videoResolution;
        private bool fullscreen = false;
        private bool streamVideo = true;
        private Task tcpLoop = null;
        private Size lastResolution;
        private Size fullscreenSize = new Size
        {
            Width = 400,
            Height = 100
        };

        private Point captureAreaTopLeft;
        private Cursor applicationSelectorCursor;

        public WindowCapture()
        {
            InitializeComponent();
        }

        private void WindowCapture_Load(object sender, EventArgs e)
        {
            applicationSelectorCursor = Cursors.Hand;

            TransparencyKey = Color.Orange;
            captureArea.BackColor = Color.Orange;
            UpdateResolutionVariables();

            toolStripTextBoxTargetPort.Text = DefaultValues.MetaStreamPort.ToString();
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
            Bitmap bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

            return bmp;
        }

        private void SendPicture(UdpClient client)
        {
            if (!client.Client.Connected || !streamVideo)
            {
                return;
            }

            Bitmap bmp = GetScreenPicture(captureArea.Location.X + Location.X, captureArea.Location.Y + Location.Y, videoResolution.Width, videoResolution.Height);
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
            videoClient.Connect(clientEndpoint.Address, DefaultValues.VideoStreamPort);
            videoClient.DontFragment = false;
        }

        private async void BeginStreamLoop()
        {
            while (streamVideo)
            {
                SendPicture(videoClient);

                await Task.Delay(50);
            }
        }

        private async Task StartServerAsync()
        {
            clientListener?.Stop();
            clientListener = new TcpListener(acceptedAddress, DefaultValues.MetaStreamPort);
            clientListener.Start();
            Log($"Server started {acceptedAddress}:{DefaultValues.MetaStreamPort}");

            metaClient = await clientListener.AcceptTcpClientAsync();
            metaStream = metaClient.GetStream();
            Log("Connection recieved...");

            tcpLoop = Task.Run(() =>
            {
                while (metaClient.Connected)
                {
                    if (metaClient.Available >= Constants.MetaFrameLength)
                    {
                        byte[] buffer = new byte[Constants.MetaFrameLength];
                        metaStream.Read(buffer, 0, Constants.MetaFrameLength);

                        string[] metapacket = Encoding.UTF8.GetString(buffer).Replace("\0", string.Empty).Split(Constants.ParameterSeparator);

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
            clientEndpoint = metaClient.Client.RemoteEndPoint as IPEndPoint;
            DialogResult diaglogResult = DialogResult.Ignore;

            await Task.Run(() =>
            {
                Log("Inbound connection, awaiting action...");
                ConnectionPrompt prompt = new ConnectionPrompt();
                prompt.IPAddress = clientEndpoint.Address.ToString();
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.TopMost = true;
                diaglogResult = prompt.ShowDialog();
            });

            switch (diaglogResult)
            {
                case DialogResult.Abort: // Block
                    Log("Blocked the following IP Address: " + clientEndpoint.Address);

                    BlockIPAddress(clientEndpoint.Address);
                    metaClient.Close();

                    await StartServerAsync();
                    return;

                case DialogResult.Ignore: // Ignore
                    Log("Ignored connection");

                    metaClient.Close();

                    await StartServerAsync();
                    return;

                case DialogResult.Yes: // Accept
                    Log("Accepting connection");

                    videoClient = new UdpClient();
                    streamVideo = true;

                    var replyAccept = Send.ConnectionReply(true, videoResolution, DefaultValues.VideoStreamPort);
                    metaStream.Write(replyAccept, 0, replyAccept.Length);

                    // videoStream = new UdpClient(Constants.VideoStreamPort); //JIWJDIAJWDJ
                    break;
                case DialogResult.No: // Deny
                    var replyDeny = Send.ConnectionReply(false, Size.Empty, 0);
                    metaStream.Write(replyDeny);

                    Log($"Told {clientEndpoint.Address} to try again another day :)");
                    metaClient.Close();

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
            if (toolStripTextBoxAcceptableHost.Text == string.Empty)
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

        /// <summary>
        /// Notifies client of resolution change.
        /// </summary>
        private void NotifyResolutionChange()
        {
            if (metaClient?.Connected == false)
            {
                Log("Not connected...");
                return;
            }

            var resChange = Send.ResolutionChange(videoResolution);
            metaStream.Write(resChange, 0, resChange.Length);
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

            NotifyResolutionChange();
        }

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