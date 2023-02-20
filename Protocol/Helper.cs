﻿using System.Text;

namespace Protocol
{
    public static class Helper
    {
        public static void PadArray(ref byte[] byteArray, int fixedLength)
        {
            byte[] writeBytes = new byte[fixedLength];
            Array.Copy(byteArray, 0, writeBytes, 0, byteArray.Length);
            byteArray = writeBytes;
        }

        /// <summary>
        /// Returns unix timestamp.
        /// </summary>
        public static long TimeStamp() => DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public static string LocalizeParameter(string localizedString, object parameter0)
        {
            StringBuilder sb = new StringBuilder(localizedString);

            sb.Replace("%0", parameter0.ToString());

            return sb.ToString();
        }

        public static string LocalizeParameter(string localizedString, object parameter0, object parameter1)
        {
            StringBuilder sb = new StringBuilder(localizedString);

            sb.Replace("%0", parameter0.ToString());
            sb.Replace("%1", parameter1.ToString());

            return sb.ToString();
        }

        public static string LocalizeParameter(string localizedString, object parameter0, object parameter1, object parameter2)
        {
            StringBuilder sb = new StringBuilder(localizedString);

            sb.Replace("%0", parameter0.ToString());
            sb.Replace("%1", parameter1.ToString());
            sb.Replace("%2", parameter2.ToString());

            return sb.ToString();
        }
    }
}