namespace WindowStreamer.Image
{
    /// <summary>
    /// Represents a digital image.
    /// </summary>
    public interface IImage
    {
        /// <summary>
        /// Gets a value indicating the size of the image.
        /// </summary>
        ImageSize Size { get; }

        /// <summary>
        /// Returns 24-bit RGB pixel values in a single array.
        /// </summary>
        /// <returns>24-bit RGB image.</returns>
        byte[] ToBytes();
    }
}
