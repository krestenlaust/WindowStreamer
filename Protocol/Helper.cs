using System.Text;

namespace Protocol
{
    public static class Helper
    {
        public static byte[] PadArray(byte[] byteArray, int fixedLength)
        {
            byte[] writeBytes = new byte[fixedLength];
            Array.Copy(byteArray, 0, writeBytes, 0, byteArray.Length);

            return writeBytes;
        }

        /// <summary>
        /// Returns unix timestamp.
        /// </summary>
        public static long TimeStamp() => DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
