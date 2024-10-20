namespace Core.Exceptions.Types;

public class AppAuthorizationException : System.Exception
{
    public AppAuthorizationException()
    {
    }

    public AppAuthorizationException(string? message) : base(message)
    {
    }

    public AppAuthorizationException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}