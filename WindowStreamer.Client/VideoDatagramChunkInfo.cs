namespace WindowStreamer.Client
{
    public readonly struct VideoDatagramChunkInfo
    {
        /// <summary>
        /// The total size of header fields, expressed as sizeof of each type.
        /// </summary>
        public static int ImageDataOffset = sizeof(ushort) + sizeof(int) + sizeof(ushort) + sizeof(ushort);

        public readonly ushort ChunkIndex;
        public readonly ushort ImageWidth;
        public readonly ushort ImageHeight;
        public readonly int TotalSizeBytes;
        public readonly int ChunkSizeBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoDatagramChunkInfo"/> struct.
        /// </summary>
        /// <param name="chunkID"></param>
        /// <param name="imageWidth"></param>
        /// <param name="imageHeight"></param>
        /// <param name="totalSizeBytes"></param>
        /// <param name="chunkSizeBytes"></param>
        public VideoDatagramChunkInfo(ushort chunkID, ushort imageWidth, ushort imageHeight, int totalSizeBytes, int chunkSizeBytes)
        {
            ChunkIndex = chunkID;
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            TotalSizeBytes = totalSizeBytes;
            ChunkSizeBytes = chunkSizeBytes;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoDatagramChunkInfo"/> struct, based on info received in datagram.
        /// </summary>
        /// <param name="datagramBuffer"></param>
        /// <param name="packetCount"></param>
        public VideoDatagramChunkInfo(byte[] datagramBuffer, int packetCount)
        {
            ChunkIndex = BitConverter.ToUInt16(datagramBuffer, 0);
            TotalSizeBytes = BitConverter.ToInt32(datagramBuffer, sizeof(ushort));
            ChunkSizeBytes = ((TotalSizeBytes - 1) / packetCount) + 1;
            ImageWidth = BitConverter.ToUInt16(datagramBuffer, sizeof(ushort) + sizeof(int));
            ImageHeight = BitConverter.ToUInt16(datagramBuffer, sizeof(ushort) + sizeof(int) + sizeof(ushort));
        }
    }
}
