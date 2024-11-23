using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CollabParty.Domain.Services.Implementations;

public class AuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly string _jwtSecret;

    public AuthService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IMapper mapper,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _mapper = mapper;
        _jwtSecret = configuration["JwtSecret"];
    }


    private string CreateAccessToken(ApplicationUser user, string sessionId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.UserName.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, sessionId),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "https://localhost:7265",
            Audience = "http://localhost:5173"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenStr = tokenHandler.WriteToken(token);

        return tokenStr;
    }

    private async Task<TokenDto> RefreshTokens(TokenDto dto)
    {
        var foundSession = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == dto.RefreshToken);
        if (foundSession == null) return new TokenDto();

        var isAccessTokenValid =
            CheckIfAccessTokenIsValid(dto.AccessToken, foundSession.RefreshToken, foundSession.SessionId);
        if (!isAccessTokenValid)
        {
            await InvalidateSession(foundSession);
            return new TokenDto();
        }

        if (!foundSession.IsValid)
            await _unitOfWork.Session.InvalidateAllUsersTokens(foundSession.UserId, foundSession.SessionId);

        if (foundSession.RefreshTokenExpiry < DateTime.UtcNow)
        {
            await InvalidateSession(foundSession);
            return new TokenDto();
        }

        var newRefreshToken = await CreateRefreshToken(foundSession.UserId, foundSession.SessionId);
        await InvalidateSession(foundSession);

        var applicationUser = await _unitOfWork.User.GetAsync(u => u.Id == foundSession.UserId);

        if (applicationUser is null)
            return new TokenDto();

        var newAccessToken = CreateAccessToken(applicationUser, foundSession.SessionId);

        return new TokenDto()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private async Task<string> CreateRefreshToken(string userId, string sessionId)
    {
        Session session = new()
        {
            IsValid = true,
            UserId = userId,
            SessionId = sessionId,
            RefreshTokenExpiry = DateTime.UtcNow.AddHours(12),
            RefreshToken = Guid.NewGuid() + "-" + Guid.NewGuid()
        };

        await _unitOfWork.Session.CreateAsync(session);

        return session.RefreshToken;
    }

    private async Task InvalidateSession(Session session)
    {
        session.IsValid = false;
        await _unitOfWork.SaveAsync();
    }

    private async Task RevokeRefreshToken(TokenDto dto)
    {
        var foundSession = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == dto.RefreshToken);

        if (foundSession is null)
            return;

        var isRefreshTokenValid = CheckIfRefreshTokenIsValid(dto.RefreshToken, foundSession);
        if (!isRefreshTokenValid) return;

        var isAccessTokenValid =
            CheckIfAccessTokenIsValid(dto.AccessToken, foundSession.UserId, foundSession.SessionId);
        if (!isAccessTokenValid) return;


        await _unitOfWork.Session.InvalidateAllUsersTokens(foundSession.UserId, foundSession.SessionId);
    }

    private bool CheckIfAccessTokenIsValid(string accessToken, string expectedUserId, string expectedSessionId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:7265/",
            ValidateAudience = true,
            ValidAudience = "http://localhost:5173/",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return false;

            var userId = principal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var sessionId = principal.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Jti)?.Value;

            return userId == expectedUserId && sessionId == expectedSessionId;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool CheckIfRefreshTokenIsValid(string refreshToken, Session session)
    {
        return session.RefreshToken == refreshToken && session.RefreshTokenExpiry > DateTime.UtcNow;
    }
}