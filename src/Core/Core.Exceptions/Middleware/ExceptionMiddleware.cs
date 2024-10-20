using System.Net.Mime;
using Core.Exceptions.Handlers;
using Core.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Exceptions.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> loggerServiceBase)
{
    private readonly HttpExceptionHandler _httpExceptionHandler = new();

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (System.Exception exception)
        {
            await HandleExceptionAsync(context.Response, exception);
            await LogException(context, exception);
        }
    }

    protected virtual Task HandleExceptionAsync(HttpResponse response, dynamic exception)
    {
        response.ContentType = MediaTypeNames.Application.Json;
        _httpExceptionHandler.Response = response;

        return  _httpExceptionHandler.HandleException(exception);
    }
    protected virtual async Task LogException(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case AppBusinessException customBusinessException:
                var message = customBusinessException.Message;
                loggerServiceBase.LogWarning(exception, message);
                break;
            case AppValidationException appValidationException:
                loggerServiceBase.LogWarning(exception,
                    $"Message: {appValidationException.Message}");
                break;
            case AppAuthorizationException appAuthorizationException:
                loggerServiceBase.LogWarning(exception,
                    $"Message: {appAuthorizationException.Message}");
                break;
            case AppAuthenticationException appAuthenticationException:
                loggerServiceBase.LogWarning(exception,
                    $"Message: {appAuthenticationException.Message}");
                break;
            case AppNotFoundEntityException appNotFoundEntityException:
                loggerServiceBase.LogWarning(exception,
                    $"Type: {appNotFoundEntityException.Type} - Id: {appNotFoundEntityException.Id} - Message: {appNotFoundEntityException.Message}");
                break;
            default:
                loggerServiceBase.LogError(exception, exception.Message);
                break;
        }
        
        await Task.CompletedTask;
    }

}