using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace WebApplicationMinimalApi8.ExceptionHandlers;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        await Results.Problem(
            title: exception.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier
            }
        ).ExecuteAsync(httpContext);

        return true;
    }
}
