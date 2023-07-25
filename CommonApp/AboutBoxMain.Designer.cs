using System.Reflection;

namespace CommonApp;

partial class AboutBoxMain
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Assembly Attribute Accessors

    public string AssemblyVersion
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }

    public string AssemblyCopyright
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    #endregion

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBoxMain));
        tableLayoutPanel = new TableLayoutPanel();
        logoPictureBox = new PictureBox();
        labelProductName = new Label();
        labelVersion = new Label();
        labelCopyright = new Label();
        labelCompanyName = new Label();
        textBoxDescription = new TextBox();
        okButton = new Button();
        tableLayoutPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)logoPictureBox).BeginInit();
        SuspendLayout();
        // 
        // tableLayoutPanel
        // 
        resources.ApplyResources(tableLayoutPanel, "tableLayoutPanel");
        tableLayoutPanel.Controls.Add(logoPictureBox, 0, 0);
        tableLayoutPanel.Controls.Add(labelProductName, 1, 0);
        tableLayoutPanel.Controls.Add(labelVersion, 1, 1);
        tableLayoutPanel.Controls.Add(labelCopyright, 1, 2);
        tableLayoutPanel.Controls.Add(labelCompanyName, 1, 3);
        tableLayoutPanel.Controls.Add(textBoxDescription, 1, 4);
        tableLayoutPanel.Controls.Add(okButton, 1, 5);
        tableLayoutPanel.Name = "tableLayoutPanel";
        // 
        // logoPictureBox
        // 
        resources.ApplyResources(logoPictureBox, "logoPictureBox");
        logoPictureBox.Image = Properties.Resources.Banner;
        logoPictureBox.Name = "logoPictureBox";
        tableLayoutPanel.SetRowSpan(logoPictureBox, 6);
        logoPictureBox.TabStop = false;
        // 
        // labelProductName
        // 
        resources.ApplyResources(labelProductName, "labelProductName");
        labelProductName.Name = "labelProductName";
        // 
        // labelVersion
        // 
        resources.ApplyResources(labelVersion, "labelVersion");
        labelVersion.Name = "labelVersion";
        // 
        // labelCopyright
        // 
        resources.ApplyResources(labelCopyright, "labelCopyright");
        labelCopyright.Name = "labelCopyright";
        // 
        // labelCompanyName
        // 
        resources.ApplyResources(labelCompanyName, "labelCompanyName");
        labelCompanyName.Name = "labelCompanyName";
        // 
        // textBoxDescription
        // 
        resources.ApplyResources(textBoxDescription, "textBoxDescription");
        textBoxDescription.Name = "textBoxDescription";
        textBoxDescription.ReadOnly = true;
        textBoxDescription.TabStop = false;
        // 
        // okButton
        // 
        resources.ApplyResources(okButton, "okButton");
        okButton.DialogResult = DialogResult.Cancel;
        okButton.Name = "okButton";
        // 
        // AboutBoxMain
        // 
        AcceptButton = okButton;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(tableLayoutPanel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutBoxMain";
        ShowIcon = false;
        ShowInTaskbar = false;
        tableLayoutPanel.ResumeLayout(false);
        tableLayoutPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)logoPictureBox).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private TableLayoutPanel tableLayoutPanel;
    private PictureBox logoPictureBox;
    private Label labelProductName;
    private Label labelVersion;
    private Label labelCopyright;
    private Label labelCompanyName;
    private TextBox textBoxDescription;
    private Button okButton;
}
