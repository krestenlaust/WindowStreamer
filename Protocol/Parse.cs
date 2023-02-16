using System.Drawing;
using Microsoft.VisualBasic;

namespace Protocol
{
    public static class Parse
    {
        public static bool ResolutionChange(string[] packet, out Size resolution)
        {
            try
            {
                string[] resolutionStrings = packet[1].Split(new char[] { Constants.SingleSeparator });
                resolution = new Size(int.Parse(resolutionStrings[0]), int.Parse(resolutionStrings[1]));
                return true;
            }
            catch
            {
                resolution = Size.Empty;
                return false;
            }
        }

        public static bool ConnectionReply(string[] packet, out bool accepted, out Size resolution, out int videoPort)
        {
            try
            {
                accepted = packet[1] == "1";

                if (!accepted)
                {
                    resolution = Size.Empty;
                    videoPort = 0;
                    return true;
                }

                string[] resolutionStrings = packet[2].Split(new char[] { Constants.SingleSeparator });
                resolution = new Size(int.Parse(resolutionStrings[0]), int.Parse(resolutionStrings[1]));
                videoPort = int.Parse(packet[3]);

                return true;
            }
            catch (Exception)
            {
                resolution = Size.Empty;
                videoPort = 0;
                accepted = false;
                return false;
            }
        }
    }
}
