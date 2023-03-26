using System.Text;

namespace WindowStreamer.Protocol;

public class PacketBuilder
{
    readonly StringBuilder sb;
    readonly int packetSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketBuilder"/> class.
    /// </summary>
    public PacketBuilder()
    {
        sb = new StringBuilder();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketBuilder"/> class, and specifies the minimum byte size of the final packet.
    /// </summary>
    /// <param name="packetSize"></param>
    public PacketBuilder(int packetSize)
    {
        sb = new StringBuilder(packetSize);
        this.packetSize = packetSize;
    }

    public void AddParameter(string value)
    {
        sb.Append(value);
        sb.Append(Constants.ParameterSeparator);
    }

    public void AddParameter(char value) => AddParameter(value.ToString());

    public void AddParameter(int value) => AddParameter(value.ToString());

    public void AddParameter(ServerPacketHeader value) => AddParameter((int)value);

    public void AddMultiParameter(params object[] subvalues)
    {
        foreach (var subvalue in subvalues)
        {
            sb.Append(subvalue);
            sb.Append(Constants.SingleSeparator);
        }

        sb.Append(Constants.ParameterSeparator);
    }

    public byte[] ToBytes()
    {
        byte[] actualBytes = Encoding.UTF8.GetBytes(sb.ToString());

        if (packetSize == 0)
        {
            return actualBytes;
        }

        byte[] bytes = new byte[packetSize];
        Array.Copy(actualBytes, 0, bytes, 0, actualBytes.Length);
        return bytes;
    }
}