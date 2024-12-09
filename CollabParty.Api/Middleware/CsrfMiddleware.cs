using CollabParty.Application.Common.Models;
using CollabParty.Domain.Interfaces;

namespace CollabParty.Api.Middleware;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory; // Inject IServiceScopeFactory
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CsrfMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/auth"))
        {
            await _next(context);
            return;
        }
        
        
        // Skip the CSRF check for GET, HEAD, and OPTIONS requests
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

        // Create a scope to resolve IUnitOfWork
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var refreshToken = context.Request.Cookies["QB-REFRESH-TOKEN"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Refresh token not found.");
                return;
            }

            var session = await unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);

            if (session != null && session.CsrfTokenExpiry < DateTime.UtcNow)
            {
                // Regenerate CSRF token if expired but session is still valid
                var newCsrfToken = new CsrfToken
                {
                    Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),
                    Expiry = DateTime.UtcNow.AddMinutes(30)
                };

                session.CsrfToken = newCsrfToken.Token;
                session.CsrfTokenExpiry = newCsrfToken.Expiry;

                await unitOfWork.SaveAsync();

                // Set new CSRF token in response cookies
                context.Response.Cookies.Append("QB-CSRF-TOKEN", newCsrfToken.Token, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = newCsrfToken.Expiry
                });

                // Optionally, add a custom header to indicate that the CSRF token has been refreshed
                context.Response.Headers["QB-CSRF-REFRESHED"] = "true";
            }
        }

        await _next(context);
    }
}