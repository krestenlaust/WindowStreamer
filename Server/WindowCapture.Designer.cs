﻿namespace Server
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowCapture));
            statusStripFooter = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabelResolution = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusLabelLatest = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripStatusFPSCounter = new System.Windows.Forms.ToolStripStatusLabel();
            toolStripHeader = new System.Windows.Forms.ToolStrip();
            toolStripTextBoxAcceptableHost = new System.Windows.Forms.ToolStripTextBox();
            toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            toolStripTextBoxTargetPort = new System.Windows.Forms.ToolStripTextBox();
            toolStripButtonActionStop = new System.Windows.Forms.ToolStripButton();
            toolStripButtonActionStart = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
            captureArea = new System.Windows.Forms.Panel();
            toolTipMain = new System.Windows.Forms.ToolTip(components);
            statusStripFooter.SuspendLayout();
            toolStripHeader.SuspendLayout();
            SuspendLayout();
            // 
            // statusStripFooter
            // 
            resources.ApplyResources(statusStripFooter, "statusStripFooter");
            statusStripFooter.ImageScalingSize = new System.Drawing.Size(20, 20);
            statusStripFooter.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabelResolution, toolStripStatusLabelLatest, toolStripStatusFPSCounter });
            statusStripFooter.Name = "statusStripFooter";
            toolTipMain.SetToolTip(statusStripFooter, resources.GetString("statusStripFooter.ToolTip"));
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
            // toolStripStatusFPSCounter
            // 
            resources.ApplyResources(toolStripStatusFPSCounter, "toolStripStatusFPSCounter");
            toolStripStatusFPSCounter.Name = "toolStripStatusFPSCounter";
            // 
            // toolStripHeader
            // 
            resources.ApplyResources(toolStripHeader, "toolStripHeader");
            toolStripHeader.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStripHeader.ImageScalingSize = new System.Drawing.Size(20, 20);
            toolStripHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripTextBoxAcceptableHost, toolStripLabel1, toolStripTextBoxTargetPort, toolStripButtonActionStop, toolStripButtonActionStart, toolStripSeparator2, toolStripButtonOptions });
            toolStripHeader.Name = "toolStripHeader";
            toolStripHeader.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            toolTipMain.SetToolTip(toolStripHeader, resources.GetString("toolStripHeader.ToolTip"));
            // 
            // toolStripTextBoxAcceptableHost
            // 
            resources.ApplyResources(toolStripTextBoxAcceptableHost, "toolStripTextBoxAcceptableHost");
            toolStripTextBoxAcceptableHost.Name = "toolStripTextBoxAcceptableHost";
            // 
            // toolStripLabel1
            // 
            resources.ApplyResources(toolStripLabel1, "toolStripLabel1");
            toolStripLabel1.Name = "toolStripLabel1";
            // 
            // toolStripTextBoxTargetPort
            // 
            resources.ApplyResources(toolStripTextBoxTargetPort, "toolStripTextBoxTargetPort");
            toolStripTextBoxTargetPort.Name = "toolStripTextBoxTargetPort";
            // 
            // toolStripButtonActionStop
            // 
            resources.ApplyResources(toolStripButtonActionStop, "toolStripButtonActionStop");
            toolStripButtonActionStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonActionStop.Name = "toolStripButtonActionStop";
            toolStripButtonActionStop.Click += toolStripButtonActionStop_Click;
            // 
            // toolStripButtonActionStart
            // 
            resources.ApplyResources(toolStripButtonActionStart, "toolStripButtonActionStart");
            toolStripButtonActionStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonActionStart.Name = "toolStripButtonActionStart";
            toolStripButtonActionStart.Click += toolStripButtonActionStart_ClickAsync;
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(toolStripSeparator2, "toolStripSeparator2");
            toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // toolStripButtonOptions
            // 
            resources.ApplyResources(toolStripButtonOptions, "toolStripButtonOptions");
            toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            toolStripButtonOptions.Name = "toolStripButtonOptions";
            toolStripButtonOptions.Click += toolStripButtonOptions_Click;
            // 
            // captureArea
            // 
            resources.ApplyResources(captureArea, "captureArea");
            captureArea.Name = "captureArea";
            toolTipMain.SetToolTip(captureArea, resources.GetString("captureArea.ToolTip"));
            // 
            // WindowCapture
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(captureArea);
            Controls.Add(toolStripHeader);
            Controls.Add(statusStripFooter);
            Name = "WindowCapture";
            toolTipMain.SetToolTip(this, resources.GetString("$this.ToolTip"));
            FormClosed += WindowCapture_FormClosed;
            Load += WindowCapture_LoadAsync;
            Resize += WindowCapture_Resize;
            statusStripFooter.ResumeLayout(false);
            statusStripFooter.PerformLayout();
            toolStripHeader.ResumeLayout(false);
            toolStripHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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

