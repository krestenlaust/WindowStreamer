using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowStreamer
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private Other.Preferences NewPreferences = Other.prefs;    

        private void Options_Load(object sender, EventArgs e)
        {
            buttonApply.Enabled = false;
        }

        public void CheckChanges()
        {
            if (NewPreferences != Other.prefs)
            {
                buttonApply.Enabled = true;
            }
        }

        private void Apply()
        {
            Other.prefs = NewPreferences;
            buttonApply.Enabled = false;
        }

        private void buttonApply_Click(object sender, EventArgs e) => Apply();

        private void buttonCancel_Click(object sender, EventArgs e) => Close();

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Apply();
            Close();
        }
    }
}
