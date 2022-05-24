using Newtonsoft.Json;
using System.Net;

namespace JwtAuthorizationApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex, HttpStatusCode.InternalServerError);
        }
    }
    private static Task HandleException(HttpContext context, Exception ex, HttpStatusCode errorCode)
    {
        var errorMessage = JsonConvert.SerializeObject(new { ex.Message });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)errorCode;

        return context.Response.WriteAsync(errorMessage);
    }
}
