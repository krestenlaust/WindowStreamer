using System.IO;
using System.Windows.Forms;
using Shared;
using System;

namespace Server
{
    public partial class LogWindow : Form
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        private void LogWindow_Load(object sender, EventArgs e)
        {
            LogStreamWriter lsw = new LogStreamWriter(richTextBoxOutput);
            Console.SetOut(lsw);
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
