using System.Reflection;

namespace CommonApp;

/// <summary>
/// About box-form, with information about the project.
/// </summary>
public partial class AboutBoxMain : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutBoxMain"/> class.
    /// </summary>
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
}
