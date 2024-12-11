using CollabParty.Application.Common.Models;
using CollabParty.Domain.Interfaces;

namespace CollabParty.Api.Middleware;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CsrfMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
    {
        _next = next;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await _next(context);
            return;
        }

        if (HttpMethods.IsGet(context.Request.Method) ||
            HttpMethods.IsHead(context.Request.Method) ||
            HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var csrfTokenFromHeader = context.Request.Headers["QB-CSRF-TOKEN"];
        var csrfTokenFromCookie = context.Request.Cookies["QB-CSRF-TOKEN"];

        if (string.IsNullOrEmpty(csrfTokenFromHeader) || string.IsNullOrEmpty(csrfTokenFromCookie))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("CSRF token is missing.");
            return;
        }

        if (csrfTokenFromHeader != csrfTokenFromCookie)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("CSRF token validation failed.");
            return;
        }

        var newCsrfToken = new CsrfToken
        {
            Token = Guid.NewGuid()+ "-" + Guid.NewGuid(),
            Expiry = DateTime.UtcNow.AddMinutes(30)
        };

        context.Response.Cookies.Append("QB-CSRF-TOKEN", newCsrfToken.Token, new CookieOptions
        {
            HttpOnly = false,  
            Secure = true,     
            SameSite = SameSiteMode.Strict, 
            Expires = newCsrfToken.Expiry  
        });

        context.Response.Headers["QB-CSRF-REFRESHED"] = "true";

        await _next(context);
    }
}
