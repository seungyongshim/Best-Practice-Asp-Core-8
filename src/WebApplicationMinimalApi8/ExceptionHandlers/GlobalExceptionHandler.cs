using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationMinimalApi8.ExceptionHandlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        await Results.Problem(
            title: exception.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            extensions:  new Dictionary<string, object?>
            {
                ["traceId"] = traceId
            }
        ).ExecuteAsync(httpContext);

        return true;
    }
}
