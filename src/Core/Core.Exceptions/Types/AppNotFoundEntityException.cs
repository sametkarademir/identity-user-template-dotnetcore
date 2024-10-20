namespace Core.Exceptions.Types;

public class AppNotFoundEntityException : System.Exception
{
    public string Type { get; set; }
    public string Id { get; set; }

    public AppNotFoundEntityException(string type, string id)
    {
        Type = type;
        Id = id;
    }

    public AppNotFoundEntityException(string? message, string type, string id)
        : base(message)
    {
        Type = type;
        Id = id;
    }

    public AppNotFoundEntityException(string? message, string type, string id, System.Exception? innerException)
        : base(message, innerException)
    {
        Type = type;
        Id = id;
    }
}