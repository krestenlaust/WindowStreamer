using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using WindowStreamer.Image;
using WindowStreamer.Image.Windows;
using WindowStreamer.Server;

namespace ServerApp;

/// <summary>
/// Inquires a given <see cref="IGetCaptureArea"/>-object to get the area of the screen to screenshot, and returns the image.
/// </summary>
[SupportedOSPlatform("windows")]
public class ScreenshotGrabber : IScreenshotQuery
{
    static readonly Brush UACPromptFillColor = Brushes.Turquoise;

    /// <inheritdoc/>
    public IImage GetImage(Location location, ImageSize captureRect)
    {
        var bmp = new Bitmap(captureRect.Width, captureRect.Height, PixelFormat.Format24bppRgb);

        Graphics g = Graphics.FromImage(bmp);
        try
        {
            // Exception happens during UAC prompts.
            g.CopyFromScreen(location.X, location.Y, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            g.FillRectangle(UACPromptFillColor, location.X, location.Y, captureRect.Width, captureRect.Height);
        }

        return new NativeImage(bmp);
    }
}
