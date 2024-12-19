// using System.Security.Claims;
// using CollabParty.Application.Common.Constants;
// using CollabParty.Application.Common.Models;
// using CollabParty.Application.Services.Interfaces;
//
// namespace CollabParty.Api.Middleware;
//
// public class CsrfMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly IHttpContextAccessor _httpContextAccessor;
//     private readonly ITokenService _tokenService;
//     private readonly ISessionService _sessionService; // For session retrieval.
//
//     public CsrfMiddleware(
//         RequestDelegate next,
//         IHttpContextAccessor httpContextAccessor,
//         ITokenService tokenService,
//         ISessionService sessionService)
//     {
//         _next = next;
//         _httpContextAccessor = httpContextAccessor;
//         _tokenService = tokenService;
//         _sessionService = sessionService;
//     }
//
//     public async Task InvokeAsync(HttpContext context)
//     {
//         if (context.Request.Path.StartsWithSegments("/api/auth"))
//         {
//             await _next(context);
//             return;
//         }
//
//         if (HttpMethods.IsGet(context.Request.Method) ||
//             HttpMethods.IsHead(context.Request.Method) ||
//             HttpMethods.IsOptions(context.Request.Method))
//         {
//             await _next(context);
//             return;
//         }
//
//         var csrfTokenFromHeader = context.Request.Headers["QB-CSRF-TOKEN"];
//         var csrfTokenFromCookie = context.Request.Cookies["QB-CSRF-TOKEN"];
//
//         if (string.IsNullOrEmpty(csrfTokenFromHeader) || string.IsNullOrEmpty(csrfTokenFromCookie) ||
//             csrfTokenFromHeader != csrfTokenFromCookie)
//         {
//             // Attempt to validate refresh token
//             var refreshToken = context.Request.Cookies[CookieNames.RefreshToken];
//             if (!string.IsNullOrEmpty(refreshToken))
//             {
//                 var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
//                 var session = await _sessionService.GetSessionByUserId(userId); // Retrieve the session from DB.
//
//                 if (session != null && _tokenService.IsRefreshTokenValid(session, refreshToken))
//                 {
//                     // Create a new CSRF token
//                     var newCsrfToken = _tokenService.CreateCsrfToken();
//
//                     context.Response.Headers["QB-CSRF-REFRESHED"] = "true";
//                     await _next(context);
//                     return;
//                 }
//             }
//
//             context.Response.StatusCode = StatusCodes.Status403Forbidden;
//             await context.Response.WriteAsync("CSRF token validation failed, and refresh token is invalid.");
//             return;
//         }
//
//         await _next(context);
//     }
// }