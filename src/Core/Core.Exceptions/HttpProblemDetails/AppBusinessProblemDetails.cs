using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Exceptions.HttpProblemDetails;

public class AppBusinessProblemDetails : ProblemDetails
{
    public AppBusinessProblemDetails(string detail, int statusCode = StatusCodes.Status400BadRequest)
    {
        Title = "Rule violation";
        Detail = detail;
        Status = statusCode;
        //Type = "https://example.com/probs/business";
    }
}
