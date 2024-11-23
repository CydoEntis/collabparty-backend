using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class SessionRepository : BaseRepository<Session>, ISessionRepository
{
    private readonly AppDbContext _db;

    public SessionRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task InvalidateAllUsersTokens(string userId, string sessionId)
    {
        await _db.Sessions.Where(session => session.UserId == userId && session.SessionId == sessionId)
            .ExecuteUpdateAsync(session => session.SetProperty(s => s.IsValid, false));
    }

    public async Task<Session> UpdateAsync(Session entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.Sessions.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}