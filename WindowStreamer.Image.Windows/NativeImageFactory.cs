using System.Runtime.Versioning;

namespace WindowStreamer.Image.Windows
{
    [SupportedOSPlatform("windows")]
    public class NativeImageFactory : IImageFactory
    {
        public IImage CreateImage(byte[] imageData, ImageSize size) => new NativeImage(imageData, size.Width, size.Height);
    }
}
