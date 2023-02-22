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

        public static string LocalizeParameter(string localizedString, object parameter0)
        {
            var sb = new StringBuilder(localizedString);

            sb.Replace("%0", parameter0.ToString());

            return sb.ToString();
        }

        public static string LocalizeParameter(string localizedString, object parameter0, object parameter1)
        {
            var sb = new StringBuilder(localizedString);

            sb.Replace("%0", parameter0.ToString());
            sb.Replace("%1", parameter1.ToString());

            return sb.ToString();
        }

        public static string LocalizeParameter(string localizedString, object parameter0, object parameter1, object parameter2)
        {
            var sb = new StringBuilder(localizedString);

            sb.Replace("%0", parameter0.ToString());
            sb.Replace("%1", parameter1.ToString());
            sb.Replace("%2", parameter2.ToString());

            return sb.ToString();
        }
    }
}
