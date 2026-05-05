using Claims.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Claims.ExceptionHandlers;

public sealed class NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not NotFoundException notFoundException)
        {
            return false;
        }

        logger.LogInformation(
            "Resource not found: {EntityName} with id '{EntityId}'.",
            notFoundException.EntityName,
            notFoundException.EntityId);

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Title = "Resource not found.",
            Status = StatusCodes.Status404NotFound,
            Detail = notFoundException.Message
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
