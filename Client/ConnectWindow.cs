using System;
using System.Net;
using System.Windows.Forms;
using Shared;

namespace Client
{
    public partial class ConnectWindow : Form
    {
        public ConnectWindow()
        {
            InitializeComponent();
        }

        public IPAddress TargetIPAddress { get; set; }

        public int TargetPort { get; set; }

        IPAddress ip;
        int port;

        void ConnectWindow_Load(object sender, EventArgs e)
        {
            textBoxTargetIPAddress.Text = "127.0.0.1";
            textBoxTargetPort.Text = DefaultValues.MetaStreamPort.ToString();
        }

        void buttonConnect_Click(object sender, EventArgs e)
        {
            if (!IPAddress.TryParse(textBoxTargetIPAddress.Text, out ip))
            {
                MessageBox.Show("IP address not valid", "Error");
                return;
            }

            if (int.TryParse(textBoxTargetPort.Text, out port))
            {
                TargetIPAddress = ip;
                TargetPort = port;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Port not valid", "Error");
                return;
            }
        }

        void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        private void ConnectWindow_KeyUp(object sender, KeyEventArgs e)
        {
        }
    }
}
