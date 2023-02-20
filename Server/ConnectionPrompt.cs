using System;
using System.Windows.Forms;

namespace Server
{
    public partial class ConnectionPrompt : Form
    {
        private readonly string connectingIP;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPrompt"/> class.
        /// </summary>
        public ConnectionPrompt(string connectingIP)
        {
            InitializeComponent();

            this.connectingIP = connectingIP;
        }

        private void ConnectionPrompt_Load(object sender, EventArgs e)
        {
            labelConnect.Text = connectingIP + " wants to connect";
        }

        private void buttonDeny_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void buttonAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }

        private void buttonBlock_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }
    }
}
