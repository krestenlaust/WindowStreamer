using System.Drawing;

namespace Protocol
{
    public static class Send
    {
        public static byte[] UDPReady(int framerateCap)
        {
            var pb = new PacketBuilder(Constants.MetaFrameLength);
            pb.AddParameter((int)ClientPacketHeader.UDPReady);
            pb.AddParameter(framerateCap);

            return pb.ToBytes();
        }

        public static byte[] ResolutionChange(Size resolution)
        {
            var pb = new PacketBuilder(Constants.MetaFrameLength);
            pb.AddParameter(ServerPacketHeader.ResolutionUpdate);
            pb.AddMultiParameter(resolution.Width, resolution.Height);

            return pb.ToBytes();
        }

        public static byte[] ConnectionReply(bool accepted, Size resolution, int videoPort)
        {
            var pb = new PacketBuilder(Constants.MetaFrameLength);
            pb.AddParameter(ServerPacketHeader.ConnectionReply);
            pb.AddParameter(accepted ? '1' : '0');

            if (accepted)
            {
                pb.AddMultiParameter(resolution.Width, resolution.Height);
                pb.AddParameter(videoPort);
            }

            return pb.ToBytes();
        }
    }
}
