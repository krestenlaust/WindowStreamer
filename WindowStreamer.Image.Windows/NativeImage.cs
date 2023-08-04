using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace WindowStreamer.Image.Windows
{
    [SupportedOSPlatform("windows")]
    public class NativeImage : IImage
    {
        public NativeImage(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        /// <summary>
        /// Instantiates a new underlying Bitmap with the imagedata.
        /// </summary>
        /// <param name="imageData">24-bit RGB pixel data.</param>
        /// <param name="size">The dimensions of the image.</param>
        public NativeImage(byte[] imageData, int width, int height)
        {
            Bitmap = new Bitmap(width, height);

            BitmapData bmpData = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            IntPtr imageDataPtr = bmpData.Scan0;

            Marshal.Copy(imageData, 0, imageDataPtr, imageData.Length);
            Bitmap.UnlockBits(bmpData);
        }

        public ImageSize Size => new ImageSize(Bitmap.Width, Bitmap.Height);

        public Bitmap Bitmap { get; private set; }

        public static implicit operator Bitmap(NativeImage img) => img.Bitmap;
        public static implicit operator NativeImage(Bitmap img) => new NativeImage(img);

        public byte[] ToBytes() => ConvertBitmapToRawPixel24bpp(Bitmap);

        static byte[] ConvertBitmapToRawPixel24bpp(Bitmap bmp)
        {
            BitmapData bmpData = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            IntPtr imageDataPtr = bmpData.Scan0;
            int byteCount = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] imageDump = new byte[byteCount];

            Marshal.Copy(imageDataPtr, imageDump, 0, byteCount);
            bmp.UnlockBits(bmpData);

            return imageDump;
        }
    }
}