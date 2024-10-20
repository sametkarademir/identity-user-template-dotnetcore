namespace Core.Exceptions.Types;

public class AppValidationException : System.Exception
{
    public IEnumerable<ValidationExceptionModel> Errors { get; }

    public AppValidationException()
        : base()
    {
        Errors = Array.Empty<ValidationExceptionModel>();
    }

    public AppValidationException(string? message)
        : base(message)
    {
        Errors = Array.Empty<ValidationExceptionModel>();
    }

    public AppValidationException(string? message, System.Exception? innerException)
        : base(message, innerException)
    {
        Errors = Array.Empty<ValidationExceptionModel>();
    }

    public AppValidationException(IEnumerable<ValidationExceptionModel> errors)
        : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    private static string BuildErrorMessage(IEnumerable<ValidationExceptionModel> errors)
    {
        IEnumerable<string> arr = errors.Select(x =>
            $"{Environment.NewLine} -- {x.Property}: {string.Join(Environment.NewLine, values: x.Errors ?? Array.Empty<string>())}"
        );
        return $"Validation failed: {string.Join(string.Empty, arr)}";
    }
}

public class ValidationExceptionModel
{
    public string? Property { get; set; }
    public IEnumerable<string>? Errors { get; set; }
}
