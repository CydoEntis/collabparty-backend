using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;

namespace CollabParty.Application.Services.Implementations;

public class SessionService : ISessionService
{
    private readonly IUnitOfWork _unitOfWork;

    public SessionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateSession(string userId, string sessionId, RefreshToken refreshToken, CsrfToken csrfToken)
    {
        var session = new Session
        {
            UserId = userId,
            SessionId = sessionId,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiry = refreshToken.Expiry,
            CsrfToken = csrfToken.Token,
            CsrfTokenExpiry = csrfToken.Expiry,
            IsValid = true
        };

        await _unitOfWork.Session.CreateAsync(session);
    }

    public async Task InvalidateSession(Session session)
    {
        session.IsValid = false;
        session.RefreshTokenExpiry = DateTime.UtcNow;
        session.CsrfTokenExpiry = DateTime.UtcNow;
        await _unitOfWork.SaveAsync();
    }

    public async Task<bool> ValidateRefreshToken(string refreshToken)
    {
        var session = await _unitOfWork.Session.GetAsync(s => s.RefreshToken == refreshToken);
        return session != null && session.RefreshTokenExpiry > DateTime.UtcNow;
    }
}