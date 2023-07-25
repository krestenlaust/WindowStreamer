using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using Serilog;
using WindowStreamer.Server;

namespace ServerApp;

internal class ConnectionHandler : IConnectionHandler
{
    public HashSet<IPAddress> BlockedIPs { get; } = new ();

    /// <inheritdoc/>
    public ConnectionReply HandleIncomingConnection(IPAddress address)
    {
        if (BlockedIPs.Contains(address))
        {
            Log.Information($"Denied blocked IP Address: {address}");
            return ConnectionReply.Close;
        }

        var prompt = new ConnectionPrompt(address.ToString())
        {
            StartPosition = FormStartPosition.CenterParent,
            TopMost = true,
        };

        switch (prompt.ShowDialog())
        {
            case DialogResult.Abort: // Block
                Log.Information($"Blocked the following IP Address: {address}");

                BlockIPAddress(address);
                return ConnectionReply.Close;

            case DialogResult.Ignore: // Ignore
                Log.Information("Ignoring connection");
                return ConnectionReply.Close;

            case DialogResult.Yes: // Accept
                Log.Information("Accepting connection");
                return ConnectionReply.Accept;

            case DialogResult.No: // Deny
                Log.Information($"Told {address} to try again another day :)");
                return ConnectionReply.Deny;
            default:
                break;
        }

        return 0;
    }

    void BlockIPAddress(IPAddress ip)
    {
        BlockedIPs.Add(ip);

        Log.Information($"Blocking IP: {ip}");
    }
}
