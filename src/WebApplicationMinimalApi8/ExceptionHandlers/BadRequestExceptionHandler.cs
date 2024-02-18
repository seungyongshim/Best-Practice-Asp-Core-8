using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApplicationMinimalApi8.ExceptionHandlers;

public sealed class BadRequestExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is BadHttpRequestException ex)
        {
            logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

            await Results.Problem(
                title: exception.Message,
                statusCode: StatusCodes.Status400BadRequest
            ).ExecuteAsync(httpContext);

            return true;
        }
        return false;
    }
}
