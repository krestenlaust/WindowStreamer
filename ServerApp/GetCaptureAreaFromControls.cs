using System.Drawing;
using System.Windows.Forms;

namespace ServerApp;

internal class GetCaptureAreaFromControls : IGetCaptureArea
{
    readonly Control mainWindow;
    readonly Control captureArea;
    readonly Control toolstripHeader;

    public GetCaptureAreaFromControls(Control mainForm, Control captureAreaPanel, Control toolstripHeader)
    {
        this.mainWindow = mainForm;
        this.captureArea = captureAreaPanel;
        this.toolstripHeader = toolstripHeader;
    }

    public Point Location => new Point(captureArea.Location.X + mainWindow.Location.X, captureArea.Location.Y + mainWindow.Location.Y + toolstripHeader.Height);

    public Size Size => captureArea.Size;
}
