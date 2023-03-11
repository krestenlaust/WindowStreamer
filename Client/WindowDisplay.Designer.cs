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
            toolStripHeader = new System.Windows.Forms.ToolStrip();
            toolStripButtonResizeToFit = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonConnect = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            statusStripFooter = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabelResolution = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabelLatest = new System.Windows.Forms.ToolStripStatusLabel();
            displayArea = new System.Windows.Forms.PictureBox();
            toolStripHeader.SuspendLayout();
            statusStripFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)displayArea).BeginInit();
            SuspendLayout();
            // 
            // toolStripHeader
            // 
            toolStripHeader.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStripHeader.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonResizeToFit, toolStripSeparator1, toolStripButtonConnect, toolStripSeparator2, toolStripButtonOptions });
            resources.ApplyResources(toolStripHeader, "toolStripHeader");
            toolStripHeader.Name = "toolStripHeader";
            // 
            // toolStripButtonResizeToFit
            // 
            toolStripButtonResizeToFit.Checked = true;
            toolStripButtonResizeToFit.CheckOnClick = true;
            toolStripButtonResizeToFit.CheckState = System.Windows.Forms.CheckState.Checked;
            toolStripButtonResizeToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(toolStripButtonResizeToFit, "toolStripButtonResizeToFit");
            toolStripButtonResizeToFit.Name = "toolStripButtonResizeToFit";
            toolStripButtonResizeToFit.Click += toolStripButtonResizeToFit_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonConnect
            // 
            toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(toolStripButtonConnect, "toolStripButtonConnect");
            toolStripButtonConnect.Name = "toolStripButtonConnect";
            toolStripButtonConnect.Click += toolStripButtonConnect_ClickAsync;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonOptions
            // 
            toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(toolStripButtonOptions, "toolStripButtonOptions");
            toolStripButtonOptions.Name = "toolStripButtonOptions";
            toolStripButtonOptions.Click += toolStripButtonOptions_Click;
            // 
            // statusStripFooter
            // 
            statusStripFooter.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStripFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelResolution, toolStripStatusLabelLatest });
            resources.ApplyResources(statusStripFooter, "statusStripFooter");
            statusStripFooter.Name = "statusStripFooter";
            // 
            // toolStripStatusLabelResolution
            // 
            toolStripStatusLabelResolution.Name = "toolStripStatusLabelResolution";
            resources.ApplyResources(toolStripStatusLabelResolution, "toolStripStatusLabelResolution");
            // 
            // toolStripStatusLabelLatest
            // 
            toolStripStatusLabelLatest.Name = "toolStripStatusLabelLatest";
            resources.ApplyResources(toolStripStatusLabelLatest, "toolStripStatusLabelLatest");
            // 
            // displayArea
            // 
            resources.ApplyResources(displayArea, "displayArea");
            displayArea.Name = "displayArea";
            displayArea.TabStop = false;
            // 
            // WindowDisplay
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(displayArea);
            Controls.Add(statusStripFooter);
            Controls.Add(toolStripHeader);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Name = "WindowDisplay";
            FormClosed += WindowDisplay_FormClosed;
            Load += WindowDisplay_Load;
            toolStripHeader.ResumeLayout(false);
            toolStripHeader.PerformLayout();
            statusStripFooter.ResumeLayout(false);
            statusStripFooter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)displayArea).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripHeader;
        private System.Windows.Forms.StatusStrip statusStripFooter;
        private System.Windows.Forms.ToolStripButton toolStripButtonResizeToFit;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLatest;
        private System.Windows.Forms.PictureBox displayArea;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResolution;
    }
}

