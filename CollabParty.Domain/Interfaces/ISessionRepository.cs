using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface ISessionRepository : IBaseRepository<Session>
{
    Task<Session> UpdateAsync(Session entity);
}