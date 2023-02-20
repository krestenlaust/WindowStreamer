using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class WindowDisplay : Form
    {
        private Size formToPanelSize;
        private MemoryStream bitmapStream;
        private Size videoResolution;

        private WindowClient windowClient;

        public WindowDisplay()
        {
            InitializeComponent();
        }

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            formToPanelSize = Size.Subtract(this.Size, displayArea.Size);
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            using var connectDialog = new ConnectWindow();

            if (connectDialog.ShowDialog() == DialogResult.OK)
            {
                if (windowClient is not null)
                {
                    windowClient.ResolutionChanged -= WindowClient_ResolutionChanged;
                    windowClient.VideoframeRecieved -= WindowClient_VideoframeRecieved;
                    // TODO: Dispose of client
                    windowClient.Dispose();
                }

                windowClient = new WindowClient(connectDialog.TargetIPAddress, connectDialog.TargetPort, Log);
                windowClient.ResolutionChanged += WindowClient_ResolutionChanged;
                windowClient.VideoframeRecieved += WindowClient_VideoframeRecieved;

                Task.Run(windowClient.ConnectToServerAsync);
            }
        }

        private void WindowClient_VideoframeRecieved(byte[] frame)
        {
            Log("Updated frame");
            bitmapStream?.Dispose();
            bitmapStream = new MemoryStream(frame);

            displayArea.Image = new Bitmap(bitmapStream);
        }

        private void WindowClient_ResolutionChanged(Size obj)
        {
            videoResolution = obj;

            if (toolStripButtonResizeToFit.Checked)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ResizeToFit();
                });
            }
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e)
        {
            if (toolStripButtonResizeToFit.Checked)
            {
                ResizeToFit();
            }
        }

        private void ResizeToFit() => ResizeDisplayArea(videoResolution);

        private void ResizeDisplayArea(Size size) => this.Size = Size.Add(formToPanelSize, size);

        private void Log(object stdout)
        {
            toolStripStatusLabelLatest.Text = stdout.ToString();
            System.Diagnostics.Debug.WriteLine("[Client] " + stdout);
        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }
    }
}
