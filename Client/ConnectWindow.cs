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

        private IPAddress ip;
        private int port;

        private void ConnectWindow_Load(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.Abort;
            textBoxTargetIPAddress.Text = "127.0.0.1";
            textBoxTargetPort.Text = DefaultValues.MetaStreamPort.ToString();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (IPAddress.TryParse(textBoxTargetIPAddress.Text, out ip))
            {
                if (int.TryParse(textBoxTargetPort.Text, out port))
                {
                    this.TargetIPAddress = ip;
                    this.TargetPort = port;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Port not valid", "Error");
                }
            }
            else
            {
                MessageBox.Show("IP address not valid", "Error");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }
    }
}
