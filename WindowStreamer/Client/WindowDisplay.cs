using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using Shared;
using Shared.Protocol;
using System.IO;

namespace Client
{
    public partial class WindowDisplay : Form
    {
        public WindowDisplay()
        {
            InitializeComponent();
        }

        public IPAddress ServerIP;
        public int ServerPort;

        public UdpClient VideoStream;
        public TcpClient MetaStream;

        private Size VideoResolution;

        private void WindowDisplay_Load(object sender, EventArgs e)
        {
            ServerPort = Constants.MetaStreamPort;

            MetaStream = new TcpClient();
            VideoStream = new UdpClient();
            
            //VideoStream = new UdpClient(Constants.VideoStreamPort);
            //VideoStream.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
        }

        private void RecievedMetaPacket(IAsyncResult res)
        {
            if (!MetaStream.Connected)
            {
                toolStripStatusLabelLatest.Text = $"Could not connect to {ServerIP.ToString()}:{ServerPort.ToString()}";
                return;
            }

            string str;
            using (NetworkStream stream = MetaStream.GetStream())
            {
                byte[] data = new byte[1024];
                using (MemoryStream ms = new MemoryStream())
                {

                    int numBytesRead;
                    while ((numBytesRead = stream.Read(data, 0, data.Length)) > 0)
                    {
                        ms.Write(data, 0, numBytesRead);


                    }
                    str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                }
            }
            Console.WriteLine(str);
        }

        private void RecievedVideoFrame(IAsyncResult res)
        {
            IPEndPoint EndPoint = new IPEndPoint(ServerIP, Constants.VideoStreamPort);
            byte[] recieved = VideoStream.EndReceive(res, ref EndPoint);

            VideoStream.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
        }

        private bool Connect()
        {
            toolStripStatusLabelLatest.Text = $"Connecting to {ServerIP.ToString()}:{ServerPort.ToString()}...";
            /*
            IPAddress ip;
            if (!IPAddress.TryParse(ipaddress, out ip))
            {
                return false;
            }*/

            if (MetaStream.Connected)
            {
                MetaStream.Close();
                MetaStream = new TcpClient();
            }

            VideoStream = new UdpClient();

            //await MetaStream.ConnectAsync(ip, Constants.MetaStreamPort);
            MetaStream.BeginConnect(ServerIP, ServerPort, new AsyncCallback(RecievedMetaPacket), null);

            VideoStream.Connect(ServerIP, Constants.VideoStreamPort);
            VideoStream.BeginReceive(new AsyncCallback(RecievedVideoFrame), null);
            return true;
        }

        private void UpdateFrame(int[] frame)
        {
            DirectBitmap bmp = new DirectBitmap(VideoResolution.Width, VideoResolution.Height)
            {
                Bits = frame
            };

            displayArea.Image = bmp.Bitmap;
        }

        private void toolStripButtonConnect_Click(object sender, EventArgs e)
        {
            using (var ConnectDialog = new ConnectWindow())
            {
                if (ConnectDialog.ShowDialog() == DialogResult.OK)
                {
                    ServerIP = ConnectDialog.TargetIPAddress;
                    ServerPort = ConnectDialog.TargetPort;
                    Connect();
                }
            }
        }

        private void toolStripButtonResizeToFit_Click(object sender, EventArgs e)
        {

        }
    }
}
