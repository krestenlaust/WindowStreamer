using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelSink;
using Serilog;
using Shared;

namespace Client
{
    public partial class WindowDisplay : Form
    {
        Size formToPanelSize;
        Size videoResolution;

        WindowClient windowClient;

        public WindowDisplay()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                .WriteTo.File("log-client.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug)
                .WriteTo.ToolStripLabel(toolStripStatusLabelLatest)
                .CreateLogger();
        }

        async void WindowDisplay_Load(object sender, EventArgs e)
        {
            formToPanelSize = Size.Subtract(Size, displayArea.Size);

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
                    case "--connect":
                    case "-c":
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
                            targetPort = DefaultValues.MetaStreamPort;
                        }

                        await StartConnectionToServer(address, targetPort).ConfigureAwait(false);
                        break;
                    default:
                        break;
                }
            }
        }

        async void toolStripButtonConnect_ClickAsync(object sender, EventArgs e)
        {
            using var connectDialog = new ConnectWindow();

            if (connectDialog.ShowDialog() == DialogResult.OK)
            {
                await StartConnectionToServer(connectDialog.TargetIPAddress, connectDialog.TargetPort);
            }
        }

        async Task StartConnectionToServer(IPAddress address, int targetPort)
        {
            if (windowClient is not null)
            {
                windowClient.ResolutionChanged -= WindowClient_ResolutionChanged;
                windowClient.NewFrame -= WindowClient_VideoframeRecieved;

                windowClient.Dispose();
            }

            windowClient = new WindowClient(address, targetPort);
            windowClient.ResolutionChanged += WindowClient_ResolutionChanged;
            windowClient.NewFrame += WindowClient_VideoframeRecieved;

            await windowClient.ConnectToServerAsync().ConfigureAwait(false);
        }

        void WindowClient_VideoframeRecieved(Bitmap bitmap)
        {
            //Log.Information($"{bitmap.Width}x{bitmap.Height}");

            displayArea.Invoke((MethodInvoker)delegate
            {
                displayArea.Image = bitmap;
            });
        }

        /*
        void WindowClient_VideoframeRecieved(byte[] frame)
        {
            //Log.Information("Updated frame");
            bitmapStream?.Dispose();
            bitmapStream = new MemoryStream((byte[])frame.Clone());

            displayArea.Image = new Bitmap(bitmapStream);
        }*/

        void WindowClient_ResolutionChanged(Size obj)
        {
            videoResolution = obj;

            if (toolStripButtonResizeToFit.Checked)
            {
                Invoke((MethodInvoker)delegate
                {
                    ResizeToFit();
                });
            }
        }

        void toolStripButtonResizeToFit_Click(object sender, EventArgs e)
        {
            Log.Debug($"Fit to content: {toolStripButtonResizeToFit.Checked}");

            if (toolStripButtonResizeToFit.Checked)
            {
                ResizeToFit();
                FormBorderStyle = FormBorderStyle.FixedSingle;
                statusStripFooter.SizingGrip = false;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                statusStripFooter.SizingGrip = true;
            }
        }

        void ResizeToFit() => ResizeDisplayArea(videoResolution);

        void ResizeDisplayArea(Size size) => Size = Size.Add(formToPanelSize, size);

        void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            Log.Debug($"Options opened");
            new Options().ShowDialog();
        }

        private void WindowDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Information("Application closing...");
            Log.CloseAndFlush();
        }
    }
}
