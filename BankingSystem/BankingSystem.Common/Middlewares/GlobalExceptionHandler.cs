using System.Net;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BankingSystem.Common.Exceptions;
using Microsoft.Extensions.Logging;

namespace BankingSystem.API.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception switch
        {
            TransactionsNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError,
        };

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            // Comment out or remove the StackTrace in production environments
            StackTrace = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError 
                         ? exception.StackTrace 
                         : string.Empty
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
