using System.Security.Claims;
using System.Text;
using CollabParty.Application.Common.Constants;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CollabParty.Application.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly ICookieService _cookieService;
    private readonly string _jwtSecret;
    private readonly string _jwtAudience;
    private readonly string _jwtIssuer;

    public TokenService(ICookieService cookieService, IConfiguration configuration)
    {
        _cookieService = cookieService;
        _jwtSecret = configuration[JwtNames.JwtSecret];
        _jwtAudience = configuration[JwtNames.JwtAudience];
        _jwtIssuer = configuration[JwtNames.JwtIssuer];
    }

    public string CreateAccessToken(string userId, string sessionId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(30);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, sessionId),
            }),
            Expires = expires,
            SigningCredentials = credentials,
            Audience = _jwtAudience,
            Issuer = _jwtIssuer
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        _cookieService.Append(CookieNames.AccessToken, token, true, expires);
        return token;
    }

    public RefreshToken CreateRefreshToken()
    {
        var expires = DateTime.UtcNow.AddHours(16);
        var token = Guid.NewGuid().ToString();

        _cookieService.Append(CookieNames.RefreshToken, token, true, expires);
        return new RefreshToken { Token = token, Expiry = expires };
    }


    public bool IsRefreshTokenValid(Session session, string refreshToken)
    {
        return session.RefreshToken == refreshToken && session.RefreshTokenExpiry > DateTime.UtcNow;
    }
}