using Core.Exceptions.Extensions;
using Core.Exceptions.HttpProblemDetails;
using Core.Exceptions.Types;
using Microsoft.AspNetCore.Http;

namespace Core.Exceptions.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    public HttpResponse Response
    {
#pragma warning disable S112 // General or reserved exceptions should never be thrown
        get => _response ?? throw new NullReferenceException(nameof(_response));
#pragma warning restore S112 // General or reserved exceptions should never be thrown
        set => _response = value;
    }

    private HttpResponse? _response;

    public override Task HandleException(AppBusinessException businessException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new AppBusinessProblemDetails(businessException.Message, businessException.StatusCode).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(AppValidationException validationException)
    {
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new AppValidationProblemDetails(validationException.Errors).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(AppAuthorizationException authorizationException)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        string details = new AppAuthorizationProblemDetails(authorizationException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(AppAuthenticationException authenticationException)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        string details = new AppAuthenticationProblemDetails(authenticationException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(AppNotFoundEntityException notFoundException)
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        string details = new AppNotFoundEntityProblemDetails(notFoundException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(System.Exception exception)
    {
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = new AppInternalServerErrorProblemDetails(exception.Message).ToJson();
        return Response.WriteAsync(details);
    }
}
