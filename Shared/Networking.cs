using System.Drawing;
using System.Net.Sockets;
using System.Text;

namespace Shared
{
    namespace Networking
    {
        public enum ServerPacketHeader
        {
            ConnectionReply = 0,
            ResolutionUpdate = 1,
        }
        public enum ClientPacketHeader
        {
            UDPReady = 2,
            Key = 3,
        }

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
                catch (System.Exception)
                {
                    resolution = Size.Empty;
                    videoPort = 0;
                    accepted = false;
                    return false;
                }
            }
        }

        public static class Send
        {
            public static void UDPReady(NetworkStream stream, int framerateCap)
            {
                StringBuilder packet = new StringBuilder();
                packet.Append((int)ClientPacketHeader.UDPReady);
                packet.Append(Constants.ParameterSeparator);

                packet.Append(framerateCap);

                byte[] bytes = Encoding.UTF8.GetBytes(packet.ToString());
                External.PadArray(ref bytes, Constants.MetaFrameLength);
                stream.Write(bytes, 0, bytes.Length);
            }

            public static void ResolutionChange(NetworkStream stream, Size resolution)
            {
                StringBuilder packet = new StringBuilder();
                packet.Append((int)ServerPacketHeader.ResolutionUpdate);
                packet.Append(Constants.ParameterSeparator);

                packet.Append(resolution.Width);
                packet.Append(Constants.SingleSeparator);
                packet.Append(resolution.Height);

                byte[] bytes = Encoding.UTF8.GetBytes(packet.ToString());
                External.PadArray(ref bytes, Constants.MetaFrameLength);
                stream.Write(bytes, 0, bytes.Length);
            }

            public static void ConnectionReply(NetworkStream stream, bool accepted, Size resolution, int videoPort)
            {
                StringBuilder packet = new StringBuilder();
                packet.Append((int)ServerPacketHeader.ConnectionReply);
                packet.Append(Constants.ParameterSeparator);

                packet.Append(accepted ? '1' : '0');

                if (accepted)
                {
                    packet.Append(Constants.ParameterSeparator);

                    packet.Append(resolution.Width);
                    packet.Append(Constants.SingleSeparator);
                    packet.Append(resolution.Height);
                    packet.Append(Constants.ParameterSeparator);

                    packet.Append(videoPort);
                }

                byte[] bytes = Encoding.UTF8.GetBytes(packet.ToString());
                External.PadArray(ref bytes, Constants.MetaFrameLength);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
