using System;
using System.IO;
using System.Windows.Forms;
using Shared;

namespace Server
{
    public partial class LogWindow : Form
    {
        public TextWriter sw;

        public LogWindow()
        {
            InitializeComponent();
        }

        void LogWindow_Load(object sender, EventArgs e)
        {
            sw = Console.Out;

            LogStreamWriter lsw = new LogStreamWriter(richTextBoxOutput);
            Console.SetOut(lsw);
        }

        void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}
