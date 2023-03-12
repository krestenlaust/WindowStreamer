namespace Server
{
    partial class WindowCapture
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowCapture));
            this.statusStripFooter = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelResolution = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelLatest = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusFPSCounter = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripHeader = new System.Windows.Forms.ToolStrip();
            this.toolStripTextBoxAcceptableHost = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxTargetPort = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButtonActionStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonActionStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureArea = new System.Windows.Forms.Panel();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.statusStripFooter.SuspendLayout();
            this.toolStripHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripFooter
            // 
            resources.ApplyResources(this.statusStripFooter, "statusStripFooter");
            this.statusStripFooter.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelResolution,
            this.toolStripStatusLabelLatest,
            this.toolStripStatusFPSCounter});
            this.statusStripFooter.Name = "statusStripFooter";
            this.toolTipMain.SetToolTip(this.statusStripFooter, resources.GetString("statusStripFooter.ToolTip"));
            // 
            // toolStripStatusLabelResolution
            // 
            resources.ApplyResources(this.toolStripStatusLabelResolution, "toolStripStatusLabelResolution");
            this.toolStripStatusLabelResolution.Name = "toolStripStatusLabelResolution";
            // 
            // toolStripStatusLabelLatest
            // 
            resources.ApplyResources(this.toolStripStatusLabelLatest, "toolStripStatusLabelLatest");
            this.toolStripStatusLabelLatest.Name = "toolStripStatusLabelLatest";
            // 
            // toolStripStatusFPSCounter
            // 
            resources.ApplyResources(this.toolStripStatusFPSCounter, "toolStripStatusFPSCounter");
            this.toolStripStatusFPSCounter.Name = "toolStripStatusFPSCounter";
            // 
            // toolStripHeader
            // 
            resources.ApplyResources(this.toolStripHeader, "toolStripHeader");
            this.toolStripHeader.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripHeader.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxAcceptableHost,
            this.toolStripLabel1,
            this.toolStripTextBoxTargetPort,
            this.toolStripButtonActionStop,
            this.toolStripButtonActionStart,
            this.toolStripSeparator2,
            this.toolStripButtonOptions,
            this.toolStripDropDownButtonHelp});
            this.toolStripHeader.Name = "toolStripHeader";
            this.toolStripHeader.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolTipMain.SetToolTip(this.toolStripHeader, resources.GetString("toolStripHeader.ToolTip"));
            // 
            // toolStripTextBoxAcceptableHost
            // 
            resources.ApplyResources(this.toolStripTextBoxAcceptableHost, "toolStripTextBoxAcceptableHost");
            this.toolStripTextBoxAcceptableHost.Name = "toolStripTextBoxAcceptableHost";
            // 
            // toolStripLabel1
            // 
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            this.toolStripLabel1.Name = "toolStripLabel1";
            // 
            // toolStripTextBoxTargetPort
            // 
            resources.ApplyResources(this.toolStripTextBoxTargetPort, "toolStripTextBoxTargetPort");
            this.toolStripTextBoxTargetPort.Name = "toolStripTextBoxTargetPort";
            // 
            // toolStripButtonActionStop
            // 
            resources.ApplyResources(this.toolStripButtonActionStop, "toolStripButtonActionStop");
            this.toolStripButtonActionStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonActionStop.Name = "toolStripButtonActionStop";
            this.toolStripButtonActionStop.Click += new System.EventHandler(this.toolStripButtonActionStop_Click);
            // 
            // toolStripButtonActionStart
            // 
            resources.ApplyResources(this.toolStripButtonActionStart, "toolStripButtonActionStart");
            this.toolStripButtonActionStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonActionStart.Name = "toolStripButtonActionStart";
            this.toolStripButtonActionStart.Click += new System.EventHandler(this.toolStripButtonActionStart_ClickAsync);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripButtonOptions
            // 
            resources.ApplyResources(this.toolStripButtonOptions, "toolStripButtonOptions");
            this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonOptions.Name = "toolStripButtonOptions";
            this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStripButtonOptions_Click);
            // 
            // toolStripDropDownButtonHelp
            // 
            resources.ApplyResources(this.toolStripDropDownButtonHelp, "toolStripDropDownButtonHelp");
            this.toolStripDropDownButtonHelp.AutoToolTip = false;
            this.toolStripDropDownButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripSeparator3,
            this.aboutToolStripMenuItem});
            this.toolStripDropDownButtonHelp.Name = "toolStripDropDownButtonHelp";
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // captureArea
            // 
            resources.ApplyResources(this.captureArea, "captureArea");
            this.captureArea.Name = "captureArea";
            this.toolTipMain.SetToolTip(this.captureArea, resources.GetString("captureArea.ToolTip"));
            // 
            // WindowCapture
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.captureArea);
            this.Controls.Add(this.toolStripHeader);
            this.Controls.Add(this.statusStripFooter);
            this.Name = "WindowCapture";
            this.toolTipMain.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WindowCapture_FormClosed);
            this.Load += new System.EventHandler(this.WindowCapture_LoadAsync);
            this.Resize += new System.EventHandler(this.WindowCapture_Resize);
            this.statusStripFooter.ResumeLayout(false);
            this.statusStripFooter.PerformLayout();
            this.toolStripHeader.ResumeLayout(false);
            this.toolStripHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripFooter;
        private System.Windows.Forms.ToolStrip toolStripHeader;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLatest;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFPSCounter;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResolution;
        private System.Windows.Forms.Panel captureArea;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxAcceptableHost;
        private System.Windows.Forms.ToolStripButton toolStripButtonActionStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxTargetPort;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonActionStop;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonHelp;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

