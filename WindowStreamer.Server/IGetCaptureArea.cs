using System.Drawing;

namespace WindowStreamer.Server;

/// <summary>
/// Returns the location of where to get a screenshot, and the dimensions of the screenshot.
/// </summary>
public interface IGetCaptureArea
{
    /// <summary>
    /// Gets the locations of where the screenshot is to be taken.
    /// </summary>
    public Point Location { get; }

    /// <summary>
    /// Gets the size of the capture area.
    /// </summary>
    public Size Size { get; }
}
