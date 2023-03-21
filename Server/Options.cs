using System;
using System.Windows.Forms;

namespace Server;

public partial class Options : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Options"/> class.
    /// </summary>
    public Options()
    {
        InitializeComponent();
    }

    void Options_Load(object sender, EventArgs e)
    {
        buttonApply.Enabled = false;
    }

    void Apply()
    {
        buttonApply.Enabled = false;
    }

    void buttonApply_Click(object sender, EventArgs e) => Apply();

    void buttonCancel_Click(object sender, EventArgs e) => Close();

    void buttonOk_Click(object sender, EventArgs e)
    {
        Apply();
        Close();
    }

    void groupBox1_MouseHover(object sender, EventArgs e)
    {
        toolTipSettings.SetToolTip(groupBoxSharedWindow, "Play local over LAN");
    }

    void checkBoxMouseClick_CheckedChanged(object sender, EventArgs e)
    {
        groupBoxMouseButtons.Enabled = checkBoxMouseClick.Checked;
    }
}
