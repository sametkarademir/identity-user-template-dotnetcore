using Core.Exceptions.Enums;

namespace Core.Exceptions.Types;

public class AppBusinessException : System.Exception
{
    public int StatusCode { get; set; }
    public AppLogLevel LogLevel { get; set; }
    
    public AppBusinessException(int statusCode, AppLogLevel logLevel)
    {
        StatusCode = statusCode;
        LogLevel = logLevel;
    }

    public AppBusinessException(string? message, int statusCode, AppLogLevel logLevel) : base(message)
    {
        StatusCode = statusCode;
        LogLevel = logLevel;
    }

    public AppBusinessException(string? message, int statusCode, AppLogLevel logLevel, System.Exception? innerException) : base(message, innerException)
    {
        StatusCode = statusCode;
        LogLevel = logLevel;
    }
}
