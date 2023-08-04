namespace WindowStreamer.Image
{
    public interface IImageFactory
    {
        /// <summary>
        /// Create an image by image data and dimensions.
        /// </summary>
        /// <param name="imageData">24-bit RGB pixel data.</param>
        /// <param name="dimensions">The dimensions of the image.</param>
        /// <returns></returns>
        IImage CreateImage(byte[] imageData, Size dimensions);
    }
}
