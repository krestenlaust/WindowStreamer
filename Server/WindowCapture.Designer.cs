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
            this.captureArea = new System.Windows.Forms.Panel();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.statusStripFooter.SuspendLayout();
            this.toolStripHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStripFooter
            // 
            this.statusStripFooter.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelResolution,
            this.toolStripStatusLabelLatest,
            this.toolStripStatusFPSCounter});
            this.statusStripFooter.Location = new System.Drawing.Point(0, 309);
            this.statusStripFooter.Name = "statusStripFooter";
            this.statusStripFooter.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusStripFooter.Size = new System.Drawing.Size(509, 22);
            this.statusStripFooter.TabIndex = 0;
            this.statusStripFooter.Text = "statusStrip1";
            // 
            // toolStripStatusLabelResolution
            // 
            this.toolStripStatusLabelResolution.Name = "toolStripStatusLabelResolution";
            this.toolStripStatusLabelResolution.Size = new System.Drawing.Size(168, 17);
            this.toolStripStatusLabelResolution.Text = "toolStripStatusLabelResolution";
            // 
            // toolStripStatusLabelLatest
            // 
            this.toolStripStatusLabelLatest.Name = "toolStripStatusLabelLatest";
            this.toolStripStatusLabelLatest.Size = new System.Drawing.Size(16, 17);
            this.toolStripStatusLabelLatest.Text = "...";
            // 
            // toolStripStatusFPSCounter
            // 
            this.toolStripStatusFPSCounter.Name = "toolStripStatusFPSCounter";
            this.toolStripStatusFPSCounter.Size = new System.Drawing.Size(29, 17);
            this.toolStripStatusFPSCounter.Text = "FPS:";
            this.toolStripStatusFPSCounter.Visible = false;
            // 
            // toolStripHeader
            // 
            this.toolStripHeader.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripHeader.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBoxAcceptableHost,
            this.toolStripLabel1,
            this.toolStripTextBoxTargetPort,
            this.toolStripButtonActionStop,
            this.toolStripButtonActionStart,
            this.toolStripSeparator2,
            this.toolStripButtonOptions});
            this.toolStripHeader.Location = new System.Drawing.Point(0, 0);
            this.toolStripHeader.Name = "toolStripHeader";
            this.toolStripHeader.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripHeader.Size = new System.Drawing.Size(509, 25);
            this.toolStripHeader.TabIndex = 1;
            this.toolStripHeader.Text = "toolStrip1";
            // 
            // toolStripTextBoxAcceptableHost
            // 
            this.toolStripTextBoxAcceptableHost.Name = "toolStripTextBoxAcceptableHost";
            this.toolStripTextBoxAcceptableHost.Size = new System.Drawing.Size(106, 25);
            this.toolStripTextBoxAcceptableHost.ToolTipText = "IP to accept connection from, leave blank for any";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(10, 22);
            this.toolStripLabel1.Text = ":";
            // 
            // toolStripTextBoxTargetPort
            // 
            this.toolStripTextBoxTargetPort.MaxLength = 5;
            this.toolStripTextBoxTargetPort.Name = "toolStripTextBoxTargetPort";
            this.toolStripTextBoxTargetPort.Size = new System.Drawing.Size(53, 25);
            // 
            // toolStripButtonActionStop
            // 
            this.toolStripButtonActionStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonActionStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonActionStop.Image")));
            this.toolStripButtonActionStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonActionStop.Name = "toolStripButtonActionStop";
            this.toolStripButtonActionStop.Size = new System.Drawing.Size(35, 22);
            this.toolStripButtonActionStop.Text = "&Stop";
            this.toolStripButtonActionStop.Click += new System.EventHandler(this.toolStripButtonActionStop_Click);
            // 
            // toolStripButtonActionStart
            // 
            this.toolStripButtonActionStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonActionStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonActionStart.Image")));
            this.toolStripButtonActionStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonActionStart.Name = "toolStripButtonActionStart";
            this.toolStripButtonActionStart.Size = new System.Drawing.Size(35, 22);
            this.toolStripButtonActionStart.Text = "&Start";
            this.toolStripButtonActionStart.Click += new System.EventHandler(this.toolStripButtonActionStart_ClickAsync);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonOptions
            // 
            this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonOptions.Enabled = false;
            this.toolStripButtonOptions.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOptions.Image")));
            this.toolStripButtonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOptions.Name = "toolStripButtonOptions";
            this.toolStripButtonOptions.Size = new System.Drawing.Size(53, 22);
            this.toolStripButtonOptions.Text = "&Options";
            this.toolStripButtonOptions.Visible = false;
            this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStripButtonOptions_Click);
            // 
            // captureArea
            // 
            this.captureArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captureArea.Location = new System.Drawing.Point(0, 25);
            this.captureArea.Name = "captureArea";
            this.captureArea.Size = new System.Drawing.Size(509, 284);
            this.captureArea.TabIndex = 2;
            // 
            // WindowCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(509, 331);
            this.Controls.Add(this.captureArea);
            this.Controls.Add(this.toolStripHeader);
            this.Controls.Add(this.statusStripFooter);
            this.Name = "WindowCapture";
            this.Text = "Window Streamer - Server";
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
    }
}

