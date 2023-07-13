namespace WindowStreamer.Server.Exceptions;

public class InstanceAlreadyInUseException : Exception
{
    public InstanceAlreadyInUseException(string msg)
        : base(msg)
    {
    }
}
