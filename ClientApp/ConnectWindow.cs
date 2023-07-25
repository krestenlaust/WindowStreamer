using System;
using System.Net;
using System.Windows.Forms;

namespace ClientApp;

public partial class ConnectWindow : Form
{
    IPAddress ip;
    int port;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectWindow"/> class.
    /// </summary>
    /// <param name="defaultIP">Placeholder IP address to display in GUI.</param>
    /// <param name="defaultPort">Placeholder port to display in GUI.</param>
    public ConnectWindow(IPAddress defaultIP, int defaultPort)
    {
        InitializeComponent();

        TargetIPAddress = defaultIP;
        TargetPort = defaultPort;
    }

    /// <summary>
    /// Gets the target IP address.
    /// </summary>
    public IPAddress TargetIPAddress { get; private set; }

    /// <summary>
    /// Gets the target port.
    /// </summary>
    public int TargetPort { get; private set; }

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

        if (!int.TryParse(textBoxTargetPort.Text, out port))
        {
            MessageBox.Show("Port not valid", "Error");
            return;
        }

        TargetIPAddress = ip;
        TargetPort = port;
        DialogResult = DialogResult.OK;
        Close();
    }

    void buttonCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Abort;
        Close();
    }
}
