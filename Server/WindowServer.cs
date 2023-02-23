﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Protocol;
using Shared;

namespace Server
{
    public class WindowServer
    {
        readonly Action<object> log;
        readonly Func<(Bitmap, Size)> obtainImage;
        readonly IPAddress boundIP;
        readonly Func<IPAddress, ConnectionReply> handleConnectionRequest;

        // TODO: Why is internetwork specified here?
        TcpClient metaClient = new TcpClient(AddressFamily.InterNetwork);
        NetworkStream metaStream;
        TcpListener clientListener;
        IPEndPoint clientEndpoint;
        UdpClient videoClient;
        bool streamVideo = true;
        Size resolution;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowServer"/> class.
        /// </summary>
        /// <param name="boundIP">IP address to listen on, usually <c>IPAddress.Any</c>.</param>
        /// <param name="startingResolution">The resolution of the window.</param>
        /// <param name="obtainImage">Handler for retrieving screenshots.</param>
        /// <param name="handleConnectionRequest">Handler for replying to connection attempts.</param>
        /// <param name="logger">Logging method.</param>
        public WindowServer(IPAddress boundIP, Size startingResolution, Func<(Bitmap, Size)> obtainImage, Func<IPAddress, ConnectionReply> handleConnectionRequest, Action<object> logger)
        {
            this.boundIP = boundIP;
            log = logger;
            this.obtainImage = obtainImage;
            this.handleConnectionRequest = handleConnectionRequest;
            resolution = startingResolution;
        }

        public enum ConnectionReply
        {
            /// <summary>
            /// Accepts connection, and initiates handshake.
            /// </summary>
            Accept,

            /// <summary>
            /// Closes connection without further notice.
            /// </summary>
            Close,

            /// <summary>
            /// Responds to connection, then closes it.
            /// </summary>
            Deny,
        }

        /// <summary>
        /// Gets a value indicating whether a connection has been initiated.
        /// </summary>
        [Obsolete("Not implemneted yet")]
        public bool Connected { get; private set; }

        public async Task StartServerAsync()
        {
            clientListener?.Stop();
            clientListener = new TcpListener(boundIP, DefaultValues.MetaStreamPort);
            clientListener.Start();
            log($"Server started {boundIP}:{DefaultValues.MetaStreamPort}");

            metaClient = await clientListener.AcceptTcpClientAsync();
            metaStream = metaClient.GetStream();
            log("Connection recieved...");

            Task.Run(MetastreamLoop);

            await HandshakeAsync();
        }

        public void UpdateResolution(Size resolution)
        {
            // Notifies client of resolution change.
            if (metaClient?.Connected == false)
            {
                log("Not connected...");
                return;
            }

            var resChange = Send.ResolutionChange(resolution);
            metaStream.Write(resChange, 0, resChange.Length);
        }

        void SendPicture(UdpClient client)
        {
            if (!client.Client.Connected || !streamVideo)
            {
                return;
            }

            (Bitmap bmp, Size resolution) = obtainImage();
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, ImageFormat.Png);
                bytes = stream.ToArray();
                log(bytes.Length);
            }

            int packetCount = 32;
            for (int i = 0; i < packetCount; i++)
            {
                client.Send(new ReadOnlySpan<byte>(
                        bytes,
                        (bytes.Length / packetCount) * i,
                        bytes.Length / packetCount));

                Thread.Sleep(1);
            }
        }

        async void BeginStreamLoop()
        {
            while (streamVideo)
            {
                SendPicture(videoClient);

                await Task.Delay(50);
            }
        }

        async Task MetastreamLoop()
        {
            while (metaClient.Connected)
            {
                byte[] buffer = new byte[Constants.MetaFrameLength];
                await metaStream.ReadAsync(buffer, 0, Constants.MetaFrameLength);

                string[] metapacket = Encoding.UTF8.GetString(buffer).TrimEnd('\0').Split(Constants.ParameterSeparator);

                switch ((ClientPacketHeader)int.Parse(metapacket[0]))
                {
                    case ClientPacketHeader.Key:
                        log($"Recieved key: {metapacket[1]}");
                        break;
                    case ClientPacketHeader.UDPReady:
                        log("Udp ready!");
                        ConnectVideoStream();
                        BeginStreamLoop();
                        break;
                    default:
                        log($"Recived this: {metapacket[0]}");
                        break;
                }
            }

            log("Connection lost... or disconnected(tcp loop)");
        }

        /// <summary>
        /// Make handshake and begin listening loop.
        /// </summary>
        /// <returns></returns>
        async Task HandshakeAsync()
        {
            clientEndpoint = metaClient.Client.RemoteEndPoint as IPEndPoint;

            log("Inbound connection, awaiting action...");
            switch (handleConnectionRequest(clientEndpoint.Address))
            {
                case ConnectionReply.Close:

                    metaClient.Close();

                    await StartServerAsync();
                    return;

                case ConnectionReply.Accept:
                    log("Accepting connection");

                    videoClient = new UdpClient();
                    streamVideo = true;

                    var replyAccept = Send.ConnectionReply(true, resolution, DefaultValues.VideoStreamPort);
                    metaStream.Write(replyAccept, 0, replyAccept.Length);

                    // videoStream = new UdpClient(Constants.VideoStreamPort); //JIWJDIAJWDJ
                    break;
                case ConnectionReply.Deny:
                    var replyDeny = Send.ConnectionReply(false, Size.Empty, 0);
                    metaStream.Write(replyDeny);

                    log($"Told {clientEndpoint.Address} to try again another day :)");
                    metaClient.Close();

                    await StartServerAsync();
                    return;
                default:
                    return;
            }
        }

        void ConnectVideoStream()
        {
            videoClient.Connect(clientEndpoint.Address, DefaultValues.VideoStreamPort);
            videoClient.DontFragment = false;
        }
    }
}
