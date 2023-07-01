namespace ClientApp;

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
        textBoxTargetIPAddress = new System.Windows.Forms.TextBox();
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        textBoxTargetPort = new System.Windows.Forms.TextBox();
        buttonConnect = new System.Windows.Forms.Button();
        buttonCancel = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // textBoxTargetIPAddress
        // 
        resources.ApplyResources(textBoxTargetIPAddress, "textBoxTargetIPAddress");
        textBoxTargetIPAddress.Name = "textBoxTargetIPAddress";
        // 
        // label1
        // 
        resources.ApplyResources(label1, "label1");
        label1.Name = "label1";
        // 
        // label2
        // 
        resources.ApplyResources(label2, "label2");
        label2.Name = "label2";
        // 
        // textBoxTargetPort
        // 
        resources.ApplyResources(textBoxTargetPort, "textBoxTargetPort");
        textBoxTargetPort.Name = "textBoxTargetPort";
        // 
        // buttonConnect
        // 
        resources.ApplyResources(buttonConnect, "buttonConnect");
        buttonConnect.BackColor = System.Drawing.Color.FromArgb(128, 128, 255);
        buttonConnect.Name = "buttonConnect";
        buttonConnect.UseVisualStyleBackColor = false;
        buttonConnect.Click += buttonConnect_Click;
        // 
        // buttonCancel
        // 
        resources.ApplyResources(buttonCancel, "buttonCancel");
        buttonCancel.BackColor = System.Drawing.Color.Red;
        buttonCancel.ForeColor = System.Drawing.SystemColors.ControlText;
        buttonCancel.Name = "buttonCancel";
        buttonCancel.UseVisualStyleBackColor = false;
        buttonCancel.Click += buttonCancel_Click;
        // 
        // ConnectWindow
        // 
        AcceptButton = buttonConnect;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        CancelButton = buttonCancel;
        Controls.Add(buttonCancel);
        Controls.Add(buttonConnect);
        Controls.Add(textBoxTargetPort);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(textBoxTargetIPAddress);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "ConnectWindow";
        Load += ConnectWindow_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TextBox textBoxTargetIPAddress;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBoxTargetPort;
    private System.Windows.Forms.Button buttonConnect;
    private System.Windows.Forms.Button buttonCancel;
}