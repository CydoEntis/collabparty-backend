using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CollabParty.Application.Services.Implementations;

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Append(string name, string value, bool httpOnly, DateTime expiry)
    {
        var options = new CookieOptions
        {
            HttpOnly = httpOnly,
            Secure = true,
            // SameSite = SameSiteMode.Strict,
            SameSite = SameSiteMode.None,
            Expires = expiry
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append(name, value, options);
    }

    public string Get(string name)
    {
        return _httpContextAccessor.HttpContext.Request.Cookies[name];
    }

    public void Delete(string name)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(name);
    }
}
