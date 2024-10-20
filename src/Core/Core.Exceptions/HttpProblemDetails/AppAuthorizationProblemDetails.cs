using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Exceptions.HttpProblemDetails;

public class AppAuthorizationProblemDetails : ProblemDetails
{
    public AppAuthorizationProblemDetails(string detail)
    {
        Title = "Authorization error";
        Detail = detail;
        Status = StatusCodes.Status403Forbidden;
        //Type = "https://example.com/probs/authorization";
    }
}