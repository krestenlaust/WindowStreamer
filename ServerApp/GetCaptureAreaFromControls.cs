using System.Windows.Forms;
using WindowStreamer.Image;
using WindowStreamer.Server;

namespace ServerApp;

public class GetCaptureAreaFromControls : IGetCaptureArea
{
    readonly Control mainForm;
    readonly Control captureAreaPanel;
    readonly Control toolstripHeader;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetCaptureAreaFromControls"/> class.
    /// </summary>
    /// <param name="mainForm">The main window of the application.</param>
    /// <param name="captureAreaPanel">The panel denoting the capture area.</param>
    /// <param name="toolstripHeader">The footer control.</param>
    public GetCaptureAreaFromControls(Control mainForm, Control captureAreaPanel, Control toolstripHeader)
    {
        this.mainForm = mainForm;
        this.captureAreaPanel = captureAreaPanel;
        this.toolstripHeader = toolstripHeader;
    }

    public Point Location => new Point(captureAreaPanel.Location.X + mainForm.Location.X, captureAreaPanel.Location.Y + mainForm.Location.Y + toolstripHeader.Height);

    public Size Size => new Size(captureAreaPanel.Size.Width, captureAreaPanel.Size.Height);
}
