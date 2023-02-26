using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabelSink;
using Serilog;

namespace Client
{
    public partial class WindowDisplay : Form
    {
        Size formToPanelSize;
        MemoryStream bitmapStream;
        Size videoResolution;

        WindowClient windowClient;

        public WindowDisplay()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log-client.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.ToolStripLabel(toolStripStatusLabelLatest)
                .CreateLogger();
        }

        void WindowDisplay_Load(object sender, EventArgs e)
        {
            formToPanelSize = Size.Subtract(Size, displayArea.Size);
        }

        void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            using var connectDialog = new ConnectWindow();

            if (connectDialog.ShowDialog() == DialogResult.OK)
            {
                if (windowClient is not null)
                {
                    windowClient.ResolutionChanged -= WindowClient_ResolutionChanged;
                    windowClient.NewFrame -= WindowClient_VideoframeRecieved;

                    // TODO: Dispose of client
                    windowClient.Dispose();
                }

                windowClient = new WindowClient(connectDialog.TargetIPAddress, connectDialog.TargetPort);
                windowClient.ResolutionChanged += WindowClient_ResolutionChanged;
                windowClient.NewFrame += WindowClient_VideoframeRecieved;

                Task.Run(windowClient.ConnectToServerAsync);
            }
        }

        void WindowClient_VideoframeRecieved(Bitmap bitmap)
        {
            Log.Information($"{bitmap.Width}x{bitmap.Height}");
            displayArea.Image = bitmap;
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
