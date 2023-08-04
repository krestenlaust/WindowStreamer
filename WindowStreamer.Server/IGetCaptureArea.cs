using WindowStreamer.Image;

namespace WindowStreamer.Server;

/// <summary>
/// Returns the location of where to get a screenshot, and the dimensions of the screenshot.
/// </summary>
public interface IGetCaptureArea
{
    /// <summary>
    /// Gets the locations of where the screenshot is to be taken.
    /// </summary>
    public Location Location { get; }

    /// <summary>
    /// Gets the size of the capture area.
    /// </summary>
    public ImageSize Size { get; }
}
