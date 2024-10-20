namespace Core.Exceptions.Types;

public class AppAuthenticationException : System.Exception
{
    public AppAuthenticationException()
    {
    }

    public AppAuthenticationException(string? message) : base(message)
    {
    }

    public AppAuthenticationException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}