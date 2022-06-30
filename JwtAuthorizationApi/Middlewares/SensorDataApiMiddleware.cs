using Microsoft.Extensions.Primitives;
using System.Net;

namespace JwtAuthorizationApi.Middlewares;

public class SensorDataApiMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKey;
    public SensorDataApiMiddleware(RequestDelegate next, string apiKey)
    {
        _apiKey = apiKey;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if(!context.Request.Headers.TryGetValue("api_key", out StringValues actualKey))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            throw new Exception($"Request headers does not contain \"api_key\" header.");
            //return;
        }
        if (actualKey != _apiKey)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            throw new Exception("Wrong api key, access denied.");
            //return;
        }
        await _next(context);
    }
}
