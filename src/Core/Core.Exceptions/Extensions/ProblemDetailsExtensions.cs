﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Core.Exceptions.Extensions;

public static class ProblemDetailsExtensions
{
    public static string ToJson<TProblemDetail>(this TProblemDetail details)
        where TProblemDetail : ProblemDetails
    {
        return JsonSerializer.Serialize(details);
    }
}
