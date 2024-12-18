using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Services.Interfaces;

public interface ISessionService
{
    Task CreateSession(string userId, string sessionId, RefreshToken refreshToken);
    Task InvalidateSession(Session session);
    Task<bool> ValidateRefreshToken(string refreshToken);
    Task<Session?> GetSessionByUserId(string userId);
}