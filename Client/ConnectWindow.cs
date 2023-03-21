using System;
using System.Net;
using System.Windows.Forms;

namespace Client;

public partial class ConnectWindow : Form
{
    public ConnectWindow(IPAddress defaultIP, int defaultPort)
    {
        InitializeComponent();

        TargetIPAddress = defaultIP;
        TargetPort = defaultPort;
    }

    public IPAddress TargetIPAddress { get; private set; }

    public int TargetPort { get; private set; }

    IPAddress ip;
    int port;

    void ConnectWindow_Load(object sender, EventArgs e)
    {
        textBoxTargetIPAddress.Text = TargetIPAddress.ToString();
        textBoxTargetPort.Text = TargetPort.ToString();
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
