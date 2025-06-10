using System.Net;
using Messenger.Identity.Core.Exceptions;

namespace Messenger.Identity.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            InvalidCredentialsException => (HttpStatusCode.Unauthorized, exception.Message),
            InvalidRefreshTokenException => (HttpStatusCode.Unauthorized, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An error occurred while processing your request."),
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            Error = message,
        };

        await context.Response.WriteAsJsonAsync(response, context.RequestAborted);
    }
}
