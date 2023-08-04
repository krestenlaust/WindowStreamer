using System.Drawing;
using System.Windows.Forms;
using WindowStreamer.Server;

namespace ServerApp;

internal class GetCaptureAreaFromControls : IGetCaptureArea
{
    readonly Control mainForm;
    readonly Control captureAreaPanel;
    readonly Control toolstripHeader;

    public GetCaptureAreaFromControls(Control mainForm, Control captureAreaPanel, Control toolstripHeader)
    {
        this.mainForm = mainForm;
        this.captureAreaPanel = captureAreaPanel;
        this.toolstripHeader = toolstripHeader;
    }

    public Point Location => new Point(captureAreaPanel.Location.X + mainForm.Location.X, captureAreaPanel.Location.Y + mainForm.Location.Y + toolstripHeader.Height);

    public Size Size => captureAreaPanel.Size;
}
