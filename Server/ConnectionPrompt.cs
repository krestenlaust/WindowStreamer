using System;
using System.Windows.Forms;

namespace Server
{
    public partial class ConnectionPrompt : Form
    {
        readonly string connectingIP;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionPrompt"/> class.
        /// </summary>
        public ConnectionPrompt(string connectingIP)
        {
            InitializeComponent();

            this.connectingIP = connectingIP;
        }

        void ConnectionPrompt_Load(object sender, EventArgs e)
        {
            labelConnect.Text = connectingIP + " wants to connect";
        }

        void buttonDeny_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        void buttonAccept_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }

        void buttonIgnore_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Ignore;
            this.Close();
        }

        void buttonBlock_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }
    }
}
