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
            this.toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonApplicationSelector = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDebug1 = new System.Windows.Forms.ToolStripButton();
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
            this.toolStripButtonConnect,
            this.toolStripSeparator2,
            this.toolStripButtonOptions,
            this.toolStripSeparator1,
            this.toolStripButtonApplicationSelector,
            this.toolStripButtonDebug1});
            this.toolStripHeader.Location = new System.Drawing.Point(0, 0);
            this.toolStripHeader.Name = "toolStripHeader";
            this.toolStripHeader.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripHeader.Size = new System.Drawing.Size(509, 27);
            this.toolStripHeader.TabIndex = 1;
            this.toolStripHeader.Text = "toolStrip1";
            // 
            // toolStripTextBoxAcceptableHost
            // 
            this.toolStripTextBoxAcceptableHost.Name = "toolStripTextBoxAcceptableHost";
            this.toolStripTextBoxAcceptableHost.Size = new System.Drawing.Size(106, 27);
            this.toolStripTextBoxAcceptableHost.ToolTipText = "IP to accept connection from, leave blank for any";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(10, 24);
            this.toolStripLabel1.Text = ":";
            // 
            // toolStripTextBoxTargetPort
            // 
            this.toolStripTextBoxTargetPort.MaxLength = 5;
            this.toolStripTextBoxTargetPort.Name = "toolStripTextBoxTargetPort";
            this.toolStripTextBoxTargetPort.ReadOnly = true;
            this.toolStripTextBoxTargetPort.Size = new System.Drawing.Size(53, 27);
            // 
            // toolStripButtonConnect
            // 
            this.toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonConnect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConnect.Image")));
            this.toolStripButtonConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConnect.Name = "toolStripButtonConnect";
            this.toolStripButtonConnect.Size = new System.Drawing.Size(35, 24);
            this.toolStripButtonConnect.Text = "&Start";
            this.toolStripButtonConnect.Click += new System.EventHandler(this.toolStripButtonConnect_ClickAsync);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonOptions
            // 
            this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonOptions.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOptions.Image")));
            this.toolStripButtonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOptions.Name = "toolStripButtonOptions";
            this.toolStripButtonOptions.Size = new System.Drawing.Size(53, 24);
            this.toolStripButtonOptions.Text = "&Options";
            this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStripButtonOptions_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonApplicationSelector
            // 
            this.toolStripButtonApplicationSelector.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonApplicationSelector.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonApplicationSelector.Image")));
            this.toolStripButtonApplicationSelector.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonApplicationSelector.Name = "toolStripButtonApplicationSelector";
            this.toolStripButtonApplicationSelector.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonApplicationSelector.Text = "toolStripButtonApplicationSelector";
            this.toolStripButtonApplicationSelector.Click += new System.EventHandler(this.toolStripButtonApplicationSelector_Click);
            // 
            // toolStripButtonDebug1
            // 
            this.toolStripButtonDebug1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDebug1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDebug1.Image")));
            this.toolStripButtonDebug1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDebug1.Name = "toolStripButtonDebug1";
            this.toolStripButtonDebug1.Size = new System.Drawing.Size(24, 24);
            this.toolStripButtonDebug1.Text = "Debug1";
            this.toolStripButtonDebug1.Click += new System.EventHandler(this.toolStripButtonDebug1_Click);
            // 
            // captureArea
            // 
            this.captureArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.captureArea.Location = new System.Drawing.Point(0, 27);
            this.captureArea.Name = "captureArea";
            this.captureArea.Size = new System.Drawing.Size(509, 282);
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
        private System.Windows.Forms.ToolStripButton toolStripButtonConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBoxTargetPort;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.ToolStripButton toolStripButtonApplicationSelector;
        private System.Windows.Forms.ToolStripButton toolStripButtonDebug1;
    }
}

