using System.Drawing;
using System.Drawing.Imaging;
using WindowStreamer.Server;

namespace ServerApp;

internal class ScreenshotGrabber : IScreenshotQuery
{
    readonly IGetCaptureArea captureArea;

    public ScreenshotGrabber(IGetCaptureArea captureArea)
    {
        this.captureArea = captureArea;
    }

    public Bitmap GetImage() => GetScreenPicture(new Rectangle(captureArea.Location, captureArea.Size));

    static Bitmap GetScreenPicture(Rectangle captureRect)
    {
        var bmp = new Bitmap(captureRect.Width, captureRect.Height, PixelFormat.Format24bppRgb);

        Graphics g = Graphics.FromImage(bmp);
        g.CopyFromScreen(captureRect.Left, captureRect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

        return bmp;
    }
}
