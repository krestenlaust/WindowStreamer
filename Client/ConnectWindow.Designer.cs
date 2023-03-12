namespace Client
{
    partial class ConnectWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectWindow));
            this.textBoxTargetIPAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTargetPort = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxTargetIPAddress
            // 
            resources.ApplyResources(this.textBoxTargetIPAddress, "textBoxTargetIPAddress");
            this.textBoxTargetIPAddress.Name = "textBoxTargetIPAddress";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxTargetPort
            // 
            resources.ApplyResources(this.textBoxTargetPort, "textBoxTargetPort");
            this.textBoxTargetPort.Name = "textBoxTargetPort";
            // 
            // buttonConnect
            // 
            resources.ApplyResources(this.buttonConnect, "buttonConnect");
            this.buttonConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.UseVisualStyleBackColor = false;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.BackColor = System.Drawing.Color.Red;
            this.buttonCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            // 
            // ConnectWindow
            // 
            this.AcceptButton = this.buttonConnect;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.textBoxTargetPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTargetIPAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "ConnectWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTargetIPAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTargetPort;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.Button buttonCancel;
    }
}