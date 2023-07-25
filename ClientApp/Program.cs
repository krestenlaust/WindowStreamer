using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace ClientApp;

[SupportedOSPlatform("windows")]
static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new WindowDisplay());
    }
}
