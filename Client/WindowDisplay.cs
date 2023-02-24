﻿using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

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

                windowClient = new WindowClient(connectDialog.TargetIPAddress, connectDialog.TargetPort, Log);
                windowClient.ResolutionChanged += WindowClient_ResolutionChanged;
                windowClient.NewFrame += WindowClient_VideoframeRecieved;

                Task.Run(windowClient.ConnectToServerAsync);
            }
        }

        void WindowClient_VideoframeRecieved(byte[] frame)
        {
            Log("Updated frame");
            bitmapStream?.Dispose();
            bitmapStream = new MemoryStream((byte[])frame.Clone());

            displayArea.Image = new Bitmap(bitmapStream);
        }

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
            if (toolStripButtonResizeToFit.Checked)
            {
                ResizeToFit();
            }
        }

        void ResizeToFit() => ResizeDisplayArea(videoResolution);

        void ResizeDisplayArea(Size size) => Size = Size.Add(formToPanelSize, size);

        void Log(object stdout)
        {
            toolStripStatusLabelLatest.GetCurrentParent().Invoke((MethodInvoker)delegate
            {
                toolStripStatusLabelLatest.Text = stdout.ToString();
            });

            System.Diagnostics.Debug.WriteLine("[Client] " + stdout);
        }

        void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            new Options().ShowDialog();
        }
    }
}
