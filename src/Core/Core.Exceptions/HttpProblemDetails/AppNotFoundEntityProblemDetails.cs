using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Exceptions.HttpProblemDetails;

public class AppNotFoundEntityProblemDetails : ProblemDetails
{
    public AppNotFoundEntityProblemDetails(string detail)
    {
        Title = "Not found entity";
        Detail = detail;
        Status = StatusCodes.Status404NotFound;
        //Type = "https://example.com/probs/notfound";
    }
}
