using System.Drawing;

namespace WindowStreamer.Image.Windows
{
    public static class ImageSizeExtensions
    {
        public static Size ToSize(this ImageSize imageSize) => new Size(imageSize.Width, imageSize.Height);

        public static ImageSize ToImageSize(this Size size) => new ImageSize(size.Width, size.Height);
    }
}
