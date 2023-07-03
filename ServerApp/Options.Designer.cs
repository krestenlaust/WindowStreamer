namespace ServerApp;

partial class Options
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        buttonApply = new System.Windows.Forms.Button();
        buttonCancel = new System.Windows.Forms.Button();
        buttonOk = new System.Windows.Forms.Button();
        tabControlMain = new System.Windows.Forms.TabControl();
        tabPageGeneral = new System.Windows.Forms.TabPage();
        groupBox4 = new System.Windows.Forms.GroupBox();
        button1 = new System.Windows.Forms.Button();
        groupBoxSharedWindow = new System.Windows.Forms.GroupBox();
        checkBox1 = new System.Windows.Forms.CheckBox();
        tabPageStreaming = new System.Windows.Forms.TabPage();
        tabPageRestrictions = new System.Windows.Forms.TabPage();
        checkBox3 = new System.Windows.Forms.CheckBox();
        checkBox2 = new System.Windows.Forms.CheckBox();
        groupBoxMouse = new System.Windows.Forms.GroupBox();
        label2 = new System.Windows.Forms.Label();
        checkBoxMouseClick = new System.Windows.Forms.CheckBox();
        checkBox4 = new System.Windows.Forms.CheckBox();
        groupBoxMouseButtons = new System.Windows.Forms.GroupBox();
        checkBox7 = new System.Windows.Forms.CheckBox();
        checkBox6 = new System.Windows.Forms.CheckBox();
        groupBoxKeys = new System.Windows.Forms.GroupBox();
        flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        groupBox3 = new System.Windows.Forms.GroupBox();
        checkedListBox3 = new System.Windows.Forms.CheckedListBox();
        groupBox1 = new System.Windows.Forms.GroupBox();
        checkedListBox1 = new System.Windows.Forms.CheckedListBox();
        groupBox2 = new System.Windows.Forms.GroupBox();
        checkedListBox2 = new System.Windows.Forms.CheckedListBox();
        label1 = new System.Windows.Forms.Label();
        tabPage1 = new System.Windows.Forms.TabPage();
        toolTipSettings = new System.Windows.Forms.ToolTip(components);
        tabControlMain.SuspendLayout();
        tabPageGeneral.SuspendLayout();
        groupBox4.SuspendLayout();
        groupBoxSharedWindow.SuspendLayout();
        tabPageRestrictions.SuspendLayout();
        groupBoxMouse.SuspendLayout();
        groupBoxMouseButtons.SuspendLayout();
        groupBoxKeys.SuspendLayout();
        flowLayoutPanel1.SuspendLayout();
        groupBox3.SuspendLayout();
        groupBox1.SuspendLayout();
        groupBox2.SuspendLayout();
        SuspendLayout();
        // 
        // buttonApply
        // 
        buttonApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        buttonApply.Location = new System.Drawing.Point(432, 345);
        buttonApply.Name = "buttonApply";
        buttonApply.Size = new System.Drawing.Size(79, 38);
        buttonApply.TabIndex = 0;
        buttonApply.Text = "&Apply";
        buttonApply.UseVisualStyleBackColor = true;
        buttonApply.Click += buttonApply_Click;
        // 
        // buttonCancel
        // 
        buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        buttonCancel.Location = new System.Drawing.Point(352, 345);
        buttonCancel.Name = "buttonCancel";
        buttonCancel.Size = new System.Drawing.Size(79, 38);
        buttonCancel.TabIndex = 2;
        buttonCancel.Text = "&Cancel";
        buttonCancel.UseVisualStyleBackColor = true;
        buttonCancel.Click += buttonCancel_Click;
        // 
        // buttonOk
        // 
        buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
        buttonOk.Location = new System.Drawing.Point(271, 345);
        buttonOk.Name = "buttonOk";
        buttonOk.Size = new System.Drawing.Size(79, 38);
        buttonOk.TabIndex = 3;
        buttonOk.Text = "&OK";
        buttonOk.UseVisualStyleBackColor = true;
        buttonOk.Click += buttonOk_Click;
        // 
        // tabControlMain
        // 
        tabControlMain.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        tabControlMain.Controls.Add(tabPageGeneral);
        tabControlMain.Controls.Add(tabPageStreaming);
        tabControlMain.Controls.Add(tabPageRestrictions);
        tabControlMain.Controls.Add(tabPage1);
        tabControlMain.Location = new System.Drawing.Point(5, 7);
        tabControlMain.Name = "tabControlMain";
        tabControlMain.SelectedIndex = 0;
        tabControlMain.Size = new System.Drawing.Size(508, 333);
        tabControlMain.TabIndex = 4;
        // 
        // tabPageGeneral
        // 
        tabPageGeneral.Controls.Add(groupBox4);
        tabPageGeneral.Controls.Add(groupBoxSharedWindow);
        tabPageGeneral.Location = new System.Drawing.Point(4, 24);
        tabPageGeneral.Name = "tabPageGeneral";
        tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
        tabPageGeneral.Size = new System.Drawing.Size(500, 305);
        tabPageGeneral.TabIndex = 0;
        tabPageGeneral.Text = "Settings";
        tabPageGeneral.UseVisualStyleBackColor = true;
        // 
        // groupBox4
        // 
        groupBox4.Controls.Add(button1);
        groupBox4.Location = new System.Drawing.Point(15, 23);
        groupBox4.Name = "groupBox4";
        groupBox4.Size = new System.Drawing.Size(175, 94);
        groupBox4.TabIndex = 1;
        groupBox4.TabStop = false;
        groupBox4.Text = "Settings for options";
        // 
        // button1
        // 
        button1.Location = new System.Drawing.Point(46, 38);
        button1.Name = "button1";
        button1.Size = new System.Drawing.Size(63, 25);
        button1.TabIndex = 0;
        button1.Text = "button1";
        button1.UseVisualStyleBackColor = true;
        // 
        // groupBoxSharedWindow
        // 
        groupBoxSharedWindow.Controls.Add(checkBox1);
        groupBoxSharedWindow.Location = new System.Drawing.Point(316, 6);
        groupBoxSharedWindow.Name = "groupBoxSharedWindow";
        groupBoxSharedWindow.Size = new System.Drawing.Size(175, 171);
        groupBoxSharedWindow.TabIndex = 0;
        groupBoxSharedWindow.TabStop = false;
        groupBoxSharedWindow.Text = "Shared Window";
        groupBoxSharedWindow.MouseHover += groupBox1_MouseHover;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Location = new System.Drawing.Point(98, -1);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new System.Drawing.Size(68, 19);
        checkBox1.TabIndex = 0;
        checkBox1.Text = "Enabled";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // tabPageStreaming
        // 
        tabPageStreaming.Location = new System.Drawing.Point(4, 24);
        tabPageStreaming.Name = "tabPageStreaming";
        tabPageStreaming.Padding = new System.Windows.Forms.Padding(3);
        tabPageStreaming.Size = new System.Drawing.Size(500, 305);
        tabPageStreaming.TabIndex = 1;
        tabPageStreaming.Text = "Streaming";
        tabPageStreaming.UseVisualStyleBackColor = true;
        // 
        // tabPageRestrictions
        // 
        tabPageRestrictions.Controls.Add(checkBox3);
        tabPageRestrictions.Controls.Add(checkBox2);
        tabPageRestrictions.Controls.Add(groupBoxMouse);
        tabPageRestrictions.Controls.Add(groupBoxKeys);
        tabPageRestrictions.Controls.Add(label1);
        tabPageRestrictions.Location = new System.Drawing.Point(4, 24);
        tabPageRestrictions.Name = "tabPageRestrictions";
        tabPageRestrictions.Padding = new System.Windows.Forms.Padding(3);
        tabPageRestrictions.Size = new System.Drawing.Size(500, 305);
        tabPageRestrictions.TabIndex = 2;
        tabPageRestrictions.Text = "Restrictions";
        tabPageRestrictions.UseVisualStyleBackColor = true;
        // 
        // checkBox3
        // 
        checkBox3.AutoSize = true;
        checkBox3.Location = new System.Drawing.Point(131, 241);
        checkBox3.Name = "checkBox3";
        checkBox3.Size = new System.Drawing.Size(128, 19);
        checkBox3.TabIndex = 6;
        checkBox3.Text = "Keyboard restrcited";
        checkBox3.UseVisualStyleBackColor = true;
        // 
        // checkBox2
        // 
        checkBox2.AutoSize = true;
        checkBox2.Location = new System.Drawing.Point(8, 241);
        checkBox2.Name = "checkBox2";
        checkBox2.Size = new System.Drawing.Size(114, 19);
        checkBox2.TabIndex = 0;
        checkBox2.Text = "Mouse restricted";
        checkBox2.UseVisualStyleBackColor = true;
        // 
        // groupBoxMouse
        // 
        groupBoxMouse.Controls.Add(label2);
        groupBoxMouse.Controls.Add(checkBoxMouseClick);
        groupBoxMouse.Controls.Add(checkBox4);
        groupBoxMouse.Controls.Add(groupBoxMouseButtons);
        groupBoxMouse.Location = new System.Drawing.Point(3, 112);
        groupBoxMouse.Name = "groupBoxMouse";
        groupBoxMouse.Size = new System.Drawing.Size(263, 124);
        groupBoxMouse.TabIndex = 5;
        groupBoxMouse.TabStop = false;
        groupBoxMouse.Text = "Mouse";
        // 
        // label2
        // 
        label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        label2.AutoSize = true;
        label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
        label2.Location = new System.Drawing.Point(4, 103);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(142, 13);
        label2.TabIndex = 7;
        label2.Text = "Selected = Enabled/Allowed";
        // 
        // checkBoxMouseClick
        // 
        checkBoxMouseClick.AutoSize = true;
        checkBoxMouseClick.Checked = true;
        checkBoxMouseClick.CheckState = System.Windows.Forms.CheckState.Checked;
        checkBoxMouseClick.Location = new System.Drawing.Point(4, 41);
        checkBoxMouseClick.Name = "checkBoxMouseClick";
        checkBoxMouseClick.Size = new System.Drawing.Size(89, 19);
        checkBoxMouseClick.TabIndex = 1;
        checkBoxMouseClick.Text = "Mouse click";
        checkBoxMouseClick.UseVisualStyleBackColor = true;
        checkBoxMouseClick.CheckedChanged += checkBoxMouseClick_CheckedChanged;
        // 
        // checkBox4
        // 
        checkBox4.AutoSize = true;
        checkBox4.Location = new System.Drawing.Point(4, 20);
        checkBox4.Name = "checkBox4";
        checkBox4.Size = new System.Drawing.Size(148, 19);
        checkBox4.TabIndex = 0;
        checkBox4.Text = "Stream mouse position";
        checkBox4.UseVisualStyleBackColor = true;
        // 
        // groupBoxMouseButtons
        // 
        groupBoxMouseButtons.Controls.Add(checkBox7);
        groupBoxMouseButtons.Controls.Add(checkBox6);
        groupBoxMouseButtons.Location = new System.Drawing.Point(17, 57);
        groupBoxMouseButtons.Margin = new System.Windows.Forms.Padding(0);
        groupBoxMouseButtons.Name = "groupBoxMouseButtons";
        groupBoxMouseButtons.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
        groupBoxMouseButtons.Size = new System.Drawing.Size(118, 39);
        groupBoxMouseButtons.TabIndex = 2;
        groupBoxMouseButtons.TabStop = false;
        // 
        // checkBox7
        // 
        checkBox7.AutoSize = true;
        checkBox7.Dock = System.Windows.Forms.DockStyle.Right;
        checkBox7.Location = new System.Drawing.Point(59, 16);
        checkBox7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
        checkBox7.Name = "checkBox7";
        checkBox7.Padding = new System.Windows.Forms.Padding(0, 0, 4, 5);
        checkBox7.Size = new System.Drawing.Size(58, 23);
        checkBox7.TabIndex = 4;
        checkBox7.Text = "Right";
        checkBox7.UseVisualStyleBackColor = true;
        // 
        // checkBox6
        // 
        checkBox6.AutoSize = true;
        checkBox6.Dock = System.Windows.Forms.DockStyle.Left;
        checkBox6.Location = new System.Drawing.Point(1, 16);
        checkBox6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
        checkBox6.Name = "checkBox6";
        checkBox6.Padding = new System.Windows.Forms.Padding(4, 0, 0, 5);
        checkBox6.Size = new System.Drawing.Size(50, 23);
        checkBox6.TabIndex = 3;
        checkBox6.Text = "Left";
        checkBox6.UseVisualStyleBackColor = true;
        // 
        // groupBoxKeys
        // 
        groupBoxKeys.Controls.Add(flowLayoutPanel1);
        groupBoxKeys.Dock = System.Windows.Forms.DockStyle.Top;
        groupBoxKeys.Location = new System.Drawing.Point(3, 3);
        groupBoxKeys.Name = "groupBoxKeys";
        groupBoxKeys.Size = new System.Drawing.Size(494, 105);
        groupBoxKeys.TabIndex = 4;
        groupBoxKeys.TabStop = false;
        groupBoxKeys.Text = "Keys";
        // 
        // flowLayoutPanel1
        // 
        flowLayoutPanel1.AutoScroll = true;
        flowLayoutPanel1.Controls.Add(groupBox3);
        flowLayoutPanel1.Controls.Add(groupBox1);
        flowLayoutPanel1.Controls.Add(groupBox2);
        flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        flowLayoutPanel1.Location = new System.Drawing.Point(3, 19);
        flowLayoutPanel1.Name = "flowLayoutPanel1";
        flowLayoutPanel1.Size = new System.Drawing.Size(488, 83);
        flowLayoutPanel1.TabIndex = 3;
        // 
        // groupBox3
        // 
        groupBox3.Controls.Add(checkedListBox3);
        groupBox3.Location = new System.Drawing.Point(3, 1);
        groupBox3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
        groupBox3.Name = "groupBox3";
        groupBox3.Size = new System.Drawing.Size(183, 92);
        groupBox3.TabIndex = 4;
        groupBox3.TabStop = false;
        groupBox3.Text = "Function-keys";
        // 
        // checkedListBox3
        // 
        checkedListBox3.Dock = System.Windows.Forms.DockStyle.Fill;
        checkedListBox3.FormattingEnabled = true;
        checkedListBox3.Items.AddRange(new object[] { "All", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12" });
        checkedListBox3.Location = new System.Drawing.Point(3, 19);
        checkedListBox3.Name = "checkedListBox3";
        checkedListBox3.Size = new System.Drawing.Size(177, 70);
        checkedListBox3.TabIndex = 0;
        // 
        // groupBox1
        // 
        groupBox1.Controls.Add(checkedListBox1);
        groupBox1.Location = new System.Drawing.Point(192, 1);
        groupBox1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
        groupBox1.Name = "groupBox1";
        groupBox1.Size = new System.Drawing.Size(183, 75);
        groupBox1.TabIndex = 2;
        groupBox1.TabStop = false;
        groupBox1.Text = "Modifiers";
        // 
        // checkedListBox1
        // 
        checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
        checkedListBox1.FormattingEnabled = true;
        checkedListBox1.Items.AddRange(new object[] { "SHIFT", "CTRL", "Windows-Key" });
        checkedListBox1.Location = new System.Drawing.Point(3, 19);
        checkedListBox1.Name = "checkedListBox1";
        checkedListBox1.Size = new System.Drawing.Size(177, 53);
        checkedListBox1.TabIndex = 0;
        // 
        // groupBox2
        // 
        groupBox2.Controls.Add(checkedListBox2);
        groupBox2.Location = new System.Drawing.Point(3, 97);
        groupBox2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
        groupBox2.Name = "groupBox2";
        groupBox2.Size = new System.Drawing.Size(183, 88);
        groupBox2.TabIndex = 3;
        groupBox2.TabStop = false;
        groupBox2.Text = "Special";
        // 
        // checkedListBox2
        // 
        checkedListBox2.Dock = System.Windows.Forms.DockStyle.Fill;
        checkedListBox2.FormattingEnabled = true;
        checkedListBox2.Items.AddRange(new object[] { "Caps Lock", "Num Lock", "Tabs", "Escape" });
        checkedListBox2.Location = new System.Drawing.Point(3, 19);
        checkedListBox2.Name = "checkedListBox2";
        checkedListBox2.Size = new System.Drawing.Size(177, 66);
        checkedListBox2.TabIndex = 0;
        // 
        // label1
        // 
        label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
        label1.AutoSize = true;
        label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
        label1.Location = new System.Drawing.Point(5, 287);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(195, 13);
        label1.TabIndex = 1;
        label1.Text = "Selected = Restricted, except otherwise";
        // 
        // tabPage1
        // 
        tabPage1.Location = new System.Drawing.Point(4, 24);
        tabPage1.Name = "tabPage1";
        tabPage1.Padding = new System.Windows.Forms.Padding(3);
        tabPage1.Size = new System.Drawing.Size(500, 305);
        tabPage1.TabIndex = 3;
        tabPage1.Text = "Presets";
        tabPage1.UseVisualStyleBackColor = true;
        // 
        // Options
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(522, 390);
        Controls.Add(tabControlMain);
        Controls.Add(buttonOk);
        Controls.Add(buttonCancel);
        Controls.Add(buttonApply);
        Name = "Options";
        Text = "WindowStreamer Settings - Server";
        Load += Options_Load;
        tabControlMain.ResumeLayout(false);
        tabPageGeneral.ResumeLayout(false);
        groupBox4.ResumeLayout(false);
        groupBoxSharedWindow.ResumeLayout(false);
        groupBoxSharedWindow.PerformLayout();
        tabPageRestrictions.ResumeLayout(false);
        tabPageRestrictions.PerformLayout();
        groupBoxMouse.ResumeLayout(false);
        groupBoxMouse.PerformLayout();
        groupBoxMouseButtons.ResumeLayout(false);
        groupBoxMouseButtons.PerformLayout();
        groupBoxKeys.ResumeLayout(false);
        flowLayoutPanel1.ResumeLayout(false);
        groupBox3.ResumeLayout(false);
        groupBox1.ResumeLayout(false);
        groupBox2.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Button buttonApply;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.Button buttonOk;
    private System.Windows.Forms.TabControl tabControlMain;
    private System.Windows.Forms.TabPage tabPageGeneral;
    private System.Windows.Forms.TabPage tabPageStreaming;
    private System.Windows.Forms.GroupBox groupBoxSharedWindow;
    private System.Windows.Forms.ToolTip toolTipSettings;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.TabPage tabPageRestrictions;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckedListBox checkedListBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.CheckedListBox checkedListBox2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.CheckedListBox checkedListBox3;
    private System.Windows.Forms.GroupBox groupBoxKeys;
    private System.Windows.Forms.GroupBox groupBoxMouse;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.CheckBox checkBox3;
    private System.Windows.Forms.CheckBox checkBoxMouseClick;
    private System.Windows.Forms.CheckBox checkBox4;
    private System.Windows.Forms.GroupBox groupBoxMouseButtons;
    private System.Windows.Forms.CheckBox checkBox7;
    private System.Windows.Forms.CheckBox checkBox6;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.Label label2;
}