using System.Drawing;
using System.Drawing.Imaging;
using WindowStreamer.Server;

namespace ServerApp;

internal class ScreenshotGrabber : IScreenshotQuery
{
    public Size VideoResolution { get; internal set; }

    public Bitmap GetScreenshot()
    {
        var size = new Size(VideoResolution.Width, VideoResolution.Height);
        return GetScreenPicture(captureArea.Location.X + Location.X, captureArea.Location.Y + Location.Y + toolStripHeader.Height, size);
    }

    static Bitmap GetScreenPicture(int x, int y, Size size)
    {
        // TODO: Debug funktionen for at oversætte skærm koordinaterne til pixels.
        /*Rectangle screen = Rectangle.Empty;
        this.Invoke((MethodInvoker)delegate
        {
            screen = Screen.FromControl(this).WorkingArea;
        });

        position.X = position.X / screen.Width; // screen.Width: 1920
        position.Y = position.Y / screen.Height; // screen.Height: 1080
        */

        var rect = new Rectangle(x, y, size.Width, size.Height);
        var bmp = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);

        Graphics g = Graphics.FromImage(bmp);
        g.CopyFromScreen(rect.Left, rect.Top, 0, 0, bmp.Size, CopyPixelOperation.SourceCopy);

        return bmp;
    }
}
