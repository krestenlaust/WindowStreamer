using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using WindowStreamer.Server;

namespace ServerApp;

/// <summary>
/// Inquires a given <see cref="IGetCaptureArea"/>-object to get the area of the screen to screenshot, and returns the image.
/// </summary>
[SupportedOSPlatform("windows")]
internal class ScreenshotGrabber : IScreenshotQuery
{
    static readonly Brush UACPromptFillColor = Brushes.Turquoise;
    readonly IGetCaptureArea captureArea;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenshotGrabber"/> class.
    /// </summary>
    /// <param name="captureArea">An object that returns the area to grab the screen in.</param>
    public ScreenshotGrabber(IGetCaptureArea captureArea)
    {
        this.captureArea = captureArea;
    }

    /// <inheritdoc/>
    public Bitmap GetImage() => GetScreenPicture(new Rectangle(captureArea.Location, captureArea.Size));

    static Bitmap GetScreenPicture(Rectangle captureRect)
    {
        var bmp = new Bitmap(captureRect.Width, captureRect.Height, PixelFormat.Format24bppRgb);

        Graphics g = Graphics.FromImage(bmp);
        try
        {
            // Exception happens during UAC prompts.
            g.CopyFromScreen(captureRect.Left, captureRect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            g.FillRectangle(UACPromptFillColor, captureRect);
        }

        return bmp;
    }
}
