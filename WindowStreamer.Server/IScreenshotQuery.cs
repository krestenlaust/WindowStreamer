using WindowStreamer.Image;

namespace WindowStreamer.Server;

/// <summary>
/// Recieves a query when ready to accept next image to be transmitted.
/// TODO: Maybe rename to ScreenshotFactory.
/// </summary>
public interface IScreenshotQuery
{
    /// <summary>
    /// Grabs the next image to transmit.
    /// </summary>
    /// <returns>The screenshot to transmit.</returns>
    /// <param name="location">The origin of the screenshot topleft corner.</param>
    /// <param name="captureRect">The area of the screen to capture.</param>
    public IImage GetImage(Point location, Size captureRect);
}
