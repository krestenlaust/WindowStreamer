namespace Client;

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
        toolStripDropDownButtonHelp = new System.Windows.Forms.ToolStripDropDownButton();
        helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
        aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
        toolStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripButtonResizeToFit, toolStripSeparator1, toolStripButtonConnect, toolStripSeparator2, toolStripButtonOptions, toolStripDropDownButtonHelp });
        resources.ApplyResources(toolStripHeader, "toolStripHeader");
        toolStripHeader.Name = "toolStripHeader";
        // 
        // toolStripButtonResizeToFit
        // 
        resources.ApplyResources(toolStripButtonResizeToFit, "toolStripButtonResizeToFit");
        toolStripButtonResizeToFit.Checked = true;
        toolStripButtonResizeToFit.CheckOnClick = true;
        toolStripButtonResizeToFit.CheckState = System.Windows.Forms.CheckState.Checked;
        toolStripButtonResizeToFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
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
        resources.ApplyResources(toolStripButtonConnect, "toolStripButtonConnect");
        toolStripButtonConnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
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
        resources.ApplyResources(toolStripButtonOptions, "toolStripButtonOptions");
        toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        toolStripButtonOptions.Name = "toolStripButtonOptions";
        // 
        // toolStripDropDownButtonHelp
        // 
        resources.ApplyResources(toolStripDropDownButtonHelp, "toolStripDropDownButtonHelp");
        toolStripDropDownButtonHelp.AutoToolTip = false;
        toolStripDropDownButtonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        toolStripDropDownButtonHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { helpToolStripMenuItem, toolStripSeparator3, aboutToolStripMenuItem });
        toolStripDropDownButtonHelp.Name = "toolStripDropDownButtonHelp";
        // 
        // helpToolStripMenuItem
        // 
        resources.ApplyResources(helpToolStripMenuItem, "helpToolStripMenuItem");
        helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        helpToolStripMenuItem.Click += helpToolStripMenuItem_Click;
        // 
        // toolStripSeparator3
        // 
        toolStripSeparator3.Name = "toolStripSeparator3";
        resources.ApplyResources(toolStripSeparator3, "toolStripSeparator3");
        // 
        // aboutToolStripMenuItem
        // 
        resources.ApplyResources(aboutToolStripMenuItem, "aboutToolStripMenuItem");
        aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
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
        resources.ApplyResources(toolStripStatusLabelResolution, "toolStripStatusLabelResolution");
        toolStripStatusLabelResolution.Name = "toolStripStatusLabelResolution";
        // 
        // toolStripStatusLabelLatest
        // 
        resources.ApplyResources(toolStripStatusLabelLatest, "toolStripStatusLabelLatest");
        toolStripStatusLabelLatest.Name = "toolStripStatusLabelLatest";
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
        Load += WindowDisplay_LoadAsync;
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
    private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonHelp;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
}

