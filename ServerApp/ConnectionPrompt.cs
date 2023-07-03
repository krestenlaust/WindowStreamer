using System;
using System.Windows.Forms;

namespace ServerApp;

public partial class ConnectionPrompt : Form
{
    readonly string connectingIP;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionPrompt"/> class.
    /// </summary>
    /// <param name="connectingIP">The IP-address associated with the client connecting.</param>
    public ConnectionPrompt(string connectingIP)
    {
        InitializeComponent();

        this.connectingIP = connectingIP;
    }

    void ConnectionPrompt_Load(object sender, EventArgs e)
    {
        labelConnect.Text = labelConnect.Text.Replace("%ip%", connectingIP);
    }

    void buttonDeny_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.No;
        Close();
    }

    void buttonAccept_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Yes;
        Close();
    }

    void buttonIgnore_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Ignore;
        Close();
    }

    void buttonBlock_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Abort;
        Close();
    }
}
