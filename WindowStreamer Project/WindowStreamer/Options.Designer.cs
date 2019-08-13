namespace Server
{
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
            this.components = new System.ComponentModel.Container();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBoxSharedWindow = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tabPageStreaming = new System.Windows.Forms.TabPage();
            this.tabPageRestrictions = new System.Windows.Forms.TabPage();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBoxMouse = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxMouseClick = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.groupBoxMouseButtons = new System.Windows.Forms.GroupBox();
            this.checkBox7 = new System.Windows.Forms.CheckBox();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.groupBoxKeys = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkedListBox3 = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.toolTipSettings = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxDebugWindow = new System.Windows.Forms.CheckBox();
            this.tabControlMain.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBoxSharedWindow.SuspendLayout();
            this.tabPageRestrictions.SuspendLayout();
            this.groupBoxMouse.SuspendLayout();
            this.groupBoxMouseButtons.SuspendLayout();
            this.groupBoxKeys.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonApply
            // 
            this.buttonApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonApply.Location = new System.Drawing.Point(494, 368);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(90, 41);
            this.buttonApply.TabIndex = 0;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(402, 368);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(90, 41);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(310, 368);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(90, 41);
            this.buttonOk.TabIndex = 3;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Controls.Add(this.tabPageGeneral);
            this.tabControlMain.Controls.Add(this.tabPageStreaming);
            this.tabControlMain.Controls.Add(this.tabPageRestrictions);
            this.tabControlMain.Controls.Add(this.tabPage1);
            this.tabControlMain.Location = new System.Drawing.Point(6, 7);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(580, 355);
            this.tabControlMain.TabIndex = 4;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.checkBoxDebugWindow);
            this.tabPageGeneral.Controls.Add(this.groupBox4);
            this.tabPageGeneral.Controls.Add(this.groupBoxSharedWindow);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(572, 326);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "Settings";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button1);
            this.groupBox4.Location = new System.Drawing.Point(17, 25);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 100);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Settings for options";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(52, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(72, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // groupBoxSharedWindow
            // 
            this.groupBoxSharedWindow.Controls.Add(this.checkBox1);
            this.groupBoxSharedWindow.Location = new System.Drawing.Point(361, 6);
            this.groupBoxSharedWindow.Name = "groupBoxSharedWindow";
            this.groupBoxSharedWindow.Size = new System.Drawing.Size(200, 182);
            this.groupBoxSharedWindow.TabIndex = 0;
            this.groupBoxSharedWindow.TabStop = false;
            this.groupBoxSharedWindow.Text = "Shared Window";
            this.groupBoxSharedWindow.MouseHover += new System.EventHandler(this.groupBox1_MouseHover);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(112, -1);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(82, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Enabled";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tabPageStreaming
            // 
            this.tabPageStreaming.Location = new System.Drawing.Point(4, 25);
            this.tabPageStreaming.Name = "tabPageStreaming";
            this.tabPageStreaming.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStreaming.Size = new System.Drawing.Size(572, 326);
            this.tabPageStreaming.TabIndex = 1;
            this.tabPageStreaming.Text = "Streaming";
            this.tabPageStreaming.UseVisualStyleBackColor = true;
            // 
            // tabPageRestrictions
            // 
            this.tabPageRestrictions.Controls.Add(this.checkBox3);
            this.tabPageRestrictions.Controls.Add(this.checkBox2);
            this.tabPageRestrictions.Controls.Add(this.groupBoxMouse);
            this.tabPageRestrictions.Controls.Add(this.groupBoxKeys);
            this.tabPageRestrictions.Controls.Add(this.label1);
            this.tabPageRestrictions.Location = new System.Drawing.Point(4, 25);
            this.tabPageRestrictions.Name = "tabPageRestrictions";
            this.tabPageRestrictions.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRestrictions.Size = new System.Drawing.Size(572, 326);
            this.tabPageRestrictions.TabIndex = 2;
            this.tabPageRestrictions.Text = "Restrictions";
            this.tabPageRestrictions.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(150, 257);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(154, 21);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "Keyboard restrcited";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(9, 257);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(135, 21);
            this.checkBox2.TabIndex = 0;
            this.checkBox2.Text = "Mouse restricted";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBoxMouse
            // 
            this.groupBoxMouse.Controls.Add(this.label2);
            this.groupBoxMouse.Controls.Add(this.checkBoxMouseClick);
            this.groupBoxMouse.Controls.Add(this.checkBox4);
            this.groupBoxMouse.Controls.Add(this.groupBoxMouseButtons);
            this.groupBoxMouse.Location = new System.Drawing.Point(3, 119);
            this.groupBoxMouse.Name = "groupBoxMouse";
            this.groupBoxMouse.Size = new System.Drawing.Size(301, 132);
            this.groupBoxMouse.TabIndex = 5;
            this.groupBoxMouse.TabStop = false;
            this.groupBoxMouse.Text = "Mouse";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(4, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Selected = Enabled/Allowed";
            // 
            // checkBoxMouseClick
            // 
            this.checkBoxMouseClick.AutoSize = true;
            this.checkBoxMouseClick.Checked = true;
            this.checkBoxMouseClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxMouseClick.Location = new System.Drawing.Point(5, 44);
            this.checkBoxMouseClick.Name = "checkBoxMouseClick";
            this.checkBoxMouseClick.Size = new System.Drawing.Size(103, 21);
            this.checkBoxMouseClick.TabIndex = 1;
            this.checkBoxMouseClick.Text = "Mouse click";
            this.checkBoxMouseClick.UseVisualStyleBackColor = true;
            this.checkBoxMouseClick.CheckedChanged += new System.EventHandler(this.checkBoxMouseClick_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(5, 21);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(174, 21);
            this.checkBox4.TabIndex = 0;
            this.checkBox4.Text = "Stream mouse position";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // groupBoxMouseButtons
            // 
            this.groupBoxMouseButtons.Controls.Add(this.checkBox7);
            this.groupBoxMouseButtons.Controls.Add(this.checkBox6);
            this.groupBoxMouseButtons.Location = new System.Drawing.Point(19, 61);
            this.groupBoxMouseButtons.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxMouseButtons.Name = "groupBoxMouseButtons";
            this.groupBoxMouseButtons.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.groupBoxMouseButtons.Size = new System.Drawing.Size(135, 42);
            this.groupBoxMouseButtons.TabIndex = 2;
            this.groupBoxMouseButtons.TabStop = false;
            // 
            // checkBox7
            // 
            this.checkBox7.AutoSize = true;
            this.checkBox7.Dock = System.Windows.Forms.DockStyle.Right;
            this.checkBox7.Location = new System.Drawing.Point(66, 15);
            this.checkBox7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
            this.checkBox7.Name = "checkBox7";
            this.checkBox7.Padding = new System.Windows.Forms.Padding(0, 0, 5, 5);
            this.checkBox7.Size = new System.Drawing.Size(68, 27);
            this.checkBox7.TabIndex = 4;
            this.checkBox7.Text = "Right";
            this.checkBox7.UseVisualStyleBackColor = true;
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBox6.Location = new System.Drawing.Point(1, 15);
            this.checkBox6.Margin = new System.Windows.Forms.Padding(3, 3, 3, 1);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 5);
            this.checkBox6.Size = new System.Drawing.Size(59, 27);
            this.checkBox6.TabIndex = 3;
            this.checkBox6.Text = "Left";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // groupBoxKeys
            // 
            this.groupBoxKeys.Controls.Add(this.flowLayoutPanel1);
            this.groupBoxKeys.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxKeys.Location = new System.Drawing.Point(3, 3);
            this.groupBoxKeys.Name = "groupBoxKeys";
            this.groupBoxKeys.Size = new System.Drawing.Size(566, 112);
            this.groupBoxKeys.TabIndex = 4;
            this.groupBoxKeys.TabStop = false;
            this.groupBoxKeys.Text = "Keys";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.Controls.Add(this.groupBox3);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.groupBox2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(560, 91);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkedListBox3);
            this.groupBox3.Location = new System.Drawing.Point(3, 1);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(209, 98);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Function-keys";
            // 
            // checkedListBox3
            // 
            this.checkedListBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox3.FormattingEnabled = true;
            this.checkedListBox3.Items.AddRange(new object[] {
            "All",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.checkedListBox3.Location = new System.Drawing.Point(3, 18);
            this.checkedListBox3.Name = "checkedListBox3";
            this.checkedListBox3.Size = new System.Drawing.Size(203, 77);
            this.checkedListBox3.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkedListBox1);
            this.groupBox1.Location = new System.Drawing.Point(218, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 80);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Modifiers";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "SHIFT",
            "CTRL",
            "Windows-Key"});
            this.checkedListBox1.Location = new System.Drawing.Point(3, 18);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(203, 59);
            this.checkedListBox1.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkedListBox2);
            this.groupBox2.Location = new System.Drawing.Point(3, 103);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(209, 94);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Special";
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Items.AddRange(new object[] {
            "Caps Lock",
            "Num Lock",
            "Tabs",
            "Escape"});
            this.checkedListBox2.Location = new System.Drawing.Point(3, 18);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(203, 73);
            this.checkedListBox2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 306);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Selected = Restricted, except otherwise";
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(572, 326);
            this.tabPage1.TabIndex = 3;
            this.tabPage1.Text = "Presets";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxDebugWindow
            // 
            this.checkBoxDebugWindow.AutoSize = true;
            this.checkBoxDebugWindow.Location = new System.Drawing.Point(6, 299);
            this.checkBoxDebugWindow.Name = "checkBoxDebugWindow";
            this.checkBoxDebugWindow.Size = new System.Drawing.Size(132, 21);
            this.checkBoxDebugWindow.TabIndex = 2;
            this.checkBoxDebugWindow.Text = "DEBUG Window";
            this.checkBoxDebugWindow.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 416);
            this.Controls.Add(this.tabControlMain);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.Name = "Options";
            this.Text = "WindowStreamer Settings - Server";
            this.Load += new System.EventHandler(this.Options_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBoxSharedWindow.ResumeLayout(false);
            this.groupBoxSharedWindow.PerformLayout();
            this.tabPageRestrictions.ResumeLayout(false);
            this.tabPageRestrictions.PerformLayout();
            this.groupBoxMouse.ResumeLayout(false);
            this.groupBoxMouse.PerformLayout();
            this.groupBoxMouseButtons.ResumeLayout(false);
            this.groupBoxMouseButtons.PerformLayout();
            this.groupBoxKeys.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.CheckBox checkBoxDebugWindow;
    }
}