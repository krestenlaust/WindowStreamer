using System.Runtime.Versioning;

namespace WindowStreamer.Image.Windows
{
    [SupportedOSPlatform("windows")]
    public class NativeImageFactory : IImageFactory
    {
        public IImage CreateImage(byte[] imageData, Size size) => new NativeImage(imageData, size.Width, size.Height);
    }
}
