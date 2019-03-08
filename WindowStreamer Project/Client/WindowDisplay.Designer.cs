namespace Client
{
    partial class WindowDisplay
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowDisplay));
            this.toolStripHeader = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonResizeToFit = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            this.statusStripFooter = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelLatest = new System.Windows.Forms.ToolStripStatusLabel();
            this.displayArea = new System.Windows.Forms.PictureBox();
            this.toolStripHeader.SuspendLayout();
            this.statusStripFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayArea)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStripHeader
            // 
            this.toolStripHeader.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripHeader.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonResizeToFit,
            this.toolStripSeparator1,
            this.toolStripButtonConnect});
            this.toolStripHeader.Location = new System.Drawing.Point(0, 0);
            this.toolStripHeader.Name = "toolStripHeader";
            this.toolStripHeader.Size = new System.Drawing.Size(800, 27);
            this.toolStripHeader.TabIndex = 0;
            this.toolStripHeader.Text = "toolStrip1";
            // 
            // toolStripButtonResizeToFit
            // 
            this.toolStripButtonResizeToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonResizeToFit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonResizeToFit.Image")));
            this.toolStripButtonResizeToFit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonResizeToFit.Name = "toolStripButtonResizeToFit";
            this.toolStripButtonResizeToFit.Size = new System.Drawing.Size(93, 24);
            this.toolStripButtonResizeToFit.Text = "Resize to Fit";
            this.toolStripButtonResizeToFit.Click += new System.EventHandler(this.toolStripButtonResizeToFit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripButtonConnect
            // 
            this.toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonConnect.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConnect.Image")));
            this.toolStripButtonConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConnect.Name = "toolStripButtonConnect";
            this.toolStripButtonConnect.Size = new System.Drawing.Size(130, 24);
            this.toolStripButtonConnect.Text = "Connect to Server";
            this.toolStripButtonConnect.Click += new System.EventHandler(this.toolStripButtonConnect_Click);
            // 
            // statusStripFooter
            // 
            this.statusStripFooter.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelLatest});
            this.statusStripFooter.Location = new System.Drawing.Point(0, 427);
            this.statusStripFooter.Name = "statusStripFooter";
            this.statusStripFooter.Size = new System.Drawing.Size(800, 25);
            this.statusStripFooter.TabIndex = 1;
            this.statusStripFooter.Text = "statusStrip1";
            // 
            // toolStripStatusLabelLatest
            // 
            this.toolStripStatusLabelLatest.Name = "toolStripStatusLabelLatest";
            this.toolStripStatusLabelLatest.Size = new System.Drawing.Size(15, 20);
            this.toolStripStatusLabelLatest.Text = "..";
            // 
            // displayArea
            // 
            this.displayArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayArea.Location = new System.Drawing.Point(0, 27);
            this.displayArea.Name = "displayArea";
            this.displayArea.Size = new System.Drawing.Size(800, 400);
            this.displayArea.TabIndex = 2;
            this.displayArea.TabStop = false;
            // 
            // WindowDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 452);
            this.Controls.Add(this.displayArea);
            this.Controls.Add(this.statusStripFooter);
            this.Controls.Add(this.toolStripHeader);
            this.Name = "WindowDisplay";
            this.Text = "Window Streamer - Client";
            this.Load += new System.EventHandler(this.WindowDisplay_Load);
            this.toolStripHeader.ResumeLayout(false);
            this.toolStripHeader.PerformLayout();
            this.statusStripFooter.ResumeLayout(false);
            this.statusStripFooter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayArea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripHeader;
        private System.Windows.Forms.StatusStrip statusStripFooter;
        private System.Windows.Forms.ToolStripButton toolStripButtonResizeToFit;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLatest;
        private System.Windows.Forms.PictureBox displayArea;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonConnect;
    }
}

