using System.Drawing;

namespace ServerApp;

internal interface IGetCaptureArea
{
    public Point Location { get; }

    public Size Size { get; }
}
