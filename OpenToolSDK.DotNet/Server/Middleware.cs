using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Builder;

namespace OpenToolSDK.DotNet.Server;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _validApiKeys;
    private const string TokenPrefix = "Bearer ";

    public AuthorizationMiddleware(RequestDelegate next, IEnumerable<string> validApiKeys)
    {
        _next = next;
        _validApiKeys = new HashSet<string>(validApiKeys);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith(TokenPrefix))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Missing or malformed authorization header");
            return;
        }

        var token = authHeader.Substring(TokenPrefix.Length);
        if (!_validApiKeys.Contains(token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Invalid authorization token");
            return;
        }
        
        await _next(context);
    }
}

public static class AuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder, IEnumerable<string> validApiKeys)
    {
        return builder.UseMiddleware<AuthorizationMiddleware>(validApiKeys);
    }
}