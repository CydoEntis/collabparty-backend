using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface ISessionRepository : IBaseRepository<Session>
{
    Task InvalidateAllUsersTokens(string userId, string sessionId);
    Task<Session> UpdateAsync(Session entity);
}