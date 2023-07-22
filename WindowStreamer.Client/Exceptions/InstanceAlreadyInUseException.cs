namespace WindowStreamer.Client.Exceptions;

public class InstanceAlreadyInUseException : Exception
{
    public InstanceAlreadyInUseException(string msg)
        : base(msg)
    {
    }
}
