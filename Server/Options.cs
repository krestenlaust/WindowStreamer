using System;
using System.Windows.Forms;

namespace Server
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private Other.Preferences NewPreferences = Other.Prefs;

        private void Options_Load(object sender, EventArgs e)
        {
            buttonApply.Enabled = false;
        }

        public void CheckChanges()
        {
            if (NewPreferences != Other.Prefs)
            {
                buttonApply.Enabled = true;
            }
        }

        private void Apply()
        {
            Other.Prefs = NewPreferences;
            buttonApply.Enabled = false;
        }

        private void buttonApply_Click(object sender, EventArgs e) => Apply();

        private void buttonCancel_Click(object sender, EventArgs e) => Close();

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Apply();
            Close();
        }

        private void groupBox1_MouseHover(object sender, EventArgs e)
        {
            toolTipSettings.SetToolTip(groupBoxSharedWindow, "Play local over LAN");
        }

        private void checkBoxMouseClick_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxMouseButtons.Enabled = checkBoxMouseClick.Checked;
        }
    }
}
