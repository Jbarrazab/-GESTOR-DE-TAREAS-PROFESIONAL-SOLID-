using System.Collections.Generic;
using MiAppTerminal1.Api.Responses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MiAppTerminal1.Api;

public static class ApiExceptionHandler
{
    public static Task HandleAsync(HttpContext context)
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var statusCode = exception switch
        {
            BadHttpRequestException badRequest => badRequest.StatusCode,
            ArgumentException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(ApiExceptionHandler));

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "An unhandled exception occurred while processing the request.");
        }
        else
        {
            logger.LogInformation("Request rejected with status code {StatusCode}.", statusCode);
        }

        return Results.Json(
            new ApiErrorResponse(statusCode, context.TraceIdentifier),
            statusCode: statusCode)
            .ExecuteAsync(context);
    }
}
