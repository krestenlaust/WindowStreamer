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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButtonHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripFooter = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelResolution = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.toolStripButtonConnect,
            this.toolStripSeparator2,
            this.toolStripButtonOptions,
            this.toolStripDropDownButtonHelp});
            resources.ApplyResources(this.toolStripHeader, "toolStripHeader");
            this.toolStripHeader.Name = "toolStripHeader";
            // 
            // toolStripButtonResizeToFit
            // 
            this.toolStripButtonResizeToFit.Checked = true;
            this.toolStripButtonResizeToFit.CheckOnClick = true;
            this.toolStripButtonResizeToFit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripButtonResizeToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonResizeToFit, "toolStripButtonResizeToFit");
            this.toolStripButtonResizeToFit.Name = "toolStripButtonResizeToFit";
            this.toolStripButtonResizeToFit.Click += new System.EventHandler(this.toolStripButtonResizeToFit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolStripButtonConnect
            // 
            this.toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonConnect, "toolStripButtonConnect");
            this.toolStripButtonConnect.Name = "toolStripButtonConnect";
            this.toolStripButtonConnect.Click += new System.EventHandler(this.toolStripButtonConnect_ClickAsync);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // toolStripButtonOptions
            // 
            this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButtonOptions, "toolStripButtonOptions");
            this.toolStripButtonOptions.Name = "toolStripButtonOptions";
            this.toolStripButtonOptions.Click += new System.EventHandler(this.toolStripButtonOptions_Click);
            // 
            // toolStripDropDownButtonHelp
            // 
            this.toolStripDropDownButtonHelp.AutoToolTip = false;
            this.toolStripDropDownButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButtonHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripSeparator3,
            this.aboutToolStripMenuItem});
            resources.ApplyResources(this.toolStripDropDownButtonHelp, "toolStripDropDownButtonHelp");
            this.toolStripDropDownButtonHelp.Name = "toolStripDropDownButtonHelp";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusStripFooter
            // 
            this.statusStripFooter.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelResolution,
            this.toolStripStatusLabelLatest});
            resources.ApplyResources(this.statusStripFooter, "statusStripFooter");
            this.statusStripFooter.Name = "statusStripFooter";
            // 
            // toolStripStatusLabelResolution
            // 
            this.toolStripStatusLabelResolution.Name = "toolStripStatusLabelResolution";
            resources.ApplyResources(this.toolStripStatusLabelResolution, "toolStripStatusLabelResolution");
            // 
            // toolStripStatusLabelLatest
            // 
            this.toolStripStatusLabelLatest.Name = "toolStripStatusLabelLatest";
            resources.ApplyResources(this.toolStripStatusLabelLatest, "toolStripStatusLabelLatest");
            // 
            // displayArea
            // 
            resources.ApplyResources(this.displayArea, "displayArea");
            this.displayArea.Name = "displayArea";
            this.displayArea.TabStop = false;
            // 
            // WindowDisplay
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.displayArea);
            this.Controls.Add(this.statusStripFooter);
            this.Controls.Add(this.toolStripHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "WindowDisplay";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.WindowDisplay_FormClosed);
            this.Load += new System.EventHandler(this.WindowDisplay_LoadAsync);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelResolution;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonHelp;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}

