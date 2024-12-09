namespace CollabParty.Api.Middleware;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;

    public CsrfMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsGet(context.Request.Method) ||
            HttpMethods.IsHead(context.Request.Method) ||
            HttpMethods.IsOptions(context.Request.Method))
        {
            await _next(context);
            return;
        }

        var csrfTokenFromHeader = context.Request.Headers["X-CSRF-TOKEN"];

        var csrfTokenFromCookie = context.Request.Cookies["CSRF-TOKEN"];

        if (string.IsNullOrEmpty(csrfTokenFromHeader) || csrfTokenFromHeader != csrfTokenFromCookie)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("CSRF token validation failed.");
            return;
        }

        await _next(context);
    }
}