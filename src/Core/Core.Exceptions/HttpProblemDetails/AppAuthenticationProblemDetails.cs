using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Exceptions.HttpProblemDetails;

public class AppAuthenticationProblemDetails : ProblemDetails
{
    public AppAuthenticationProblemDetails(string detail)
    {
        Title = "Authentication error";
        Detail = detail;
        Status = StatusCodes.Status401Unauthorized;
        //Type = "https://example.com/probs/authentication";
    }
}