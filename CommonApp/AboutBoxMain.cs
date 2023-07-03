using System.Reflection;

namespace CommonApp;

public partial class AboutBoxMain : Form
{
    public AboutBoxMain()
    {
        InitializeComponent();
        this.Text = Text.Replace("%name%", ProjectProperties.Title);
        this.labelProductName.Text = labelProductName.Text.Replace("%name%", ProjectProperties.Title);
        this.labelVersion.Text = labelVersion.Text.Replace("%version%", AssemblyVersion);
        this.labelCopyright.Text = labelCopyright.Text.Replace("%copyright%", AssemblyCopyright);
        this.labelCompanyName.Text = labelCompanyName.Text.Replace("%author%", ProjectProperties.Author);
        this.textBoxDescription.Text = textBoxDescription.Text.Replace("%url%", ProjectProperties.GithubUrl).Replace("%name%", ProjectProperties.Title);
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
}
