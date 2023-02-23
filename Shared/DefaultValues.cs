using System.Drawing.Imaging;

namespace Shared
{
    public static class DefaultValues
    {
        public const int VideoStreamPort = 10064;
        public const int MetaStreamPort = 10063;
        public const int FramerateCap = 10;
        public static readonly PixelFormat ImageFormat = PixelFormat.Format24bppRgb;
        public const int BytesPerPixel = 3;
    }
}
