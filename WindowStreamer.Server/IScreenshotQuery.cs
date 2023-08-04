using System.Drawing;

namespace WindowStreamer.Server;

/// <summary>
/// Recieves a query when ready to accept next image to be transmitted.
/// </summary>
public interface IScreenshotQuery
{
    /// <summary>
    /// Grabs the next image to transmit.
    /// </summary>
    /// <returns>The screenshot to transmit.</returns>
    /// <param name="captureRect">The area of the screen to capture.</param>
    public Bitmap GetImage(Rectangle captureRect);
}
