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
    public Bitmap GetImage();
}
