using System.Drawing.Imaging;

namespace Shared
{
    public static class DefaultValues
    {
        public static readonly int VideoStreamPort = 10064;
        public static readonly int MetaStreamPort = 10063;
        public static readonly int FramerateCap = 10;
        public static readonly PixelFormat ImageFormat = PixelFormat.Format24bppRgb;
        public static readonly int BytesPerPixel = 3;
    }
}
