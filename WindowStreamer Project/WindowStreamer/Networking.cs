using Shared;
using Shared.Networking.Protocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class Networking
    {
        public static void ResolutionChange(NetworkStream stream, Size resolution)
        {
            byte[] packet = Encoding.UTF8.GetBytes((int)MetaHeader.ResolutionUpdate + "," + resolution.Width + Constants.SingleSeparator + resolution.Height);

            stream.Write(packet, 0, packet.Length);
        }
    }
}
