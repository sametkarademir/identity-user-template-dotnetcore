using Core.Exceptions.Types;

namespace Core.Exceptions.Handlers;

public abstract class ExceptionHandler
{
    public abstract Task HandleException(AppBusinessException businessException);
    public abstract Task HandleException(AppValidationException validationException);
    public abstract Task HandleException(AppAuthorizationException authorizationException);
    public abstract Task HandleException(AppAuthenticationException authenticationException);
    public abstract Task HandleException(AppNotFoundEntityException notFoundException);
    public abstract Task HandleException(System.Exception exception);
}
