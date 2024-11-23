using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        var sessions = await _db.Sessions
            .Where(session => session.UserId == userId && session.SessionId == sessionId)
            .ToListAsync();

        if (sessions.Any())
        {
            await _db.Sessions
                .Where(session => session.UserId == userId && session.SessionId == sessionId)
                .ExecuteUpdateAsync(session => session.SetProperty(s => s.IsValid, false));

            await _db.SaveChangesAsync();
        }
    }

    public async Task<Session> UpdateAsync(Session entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.Sessions.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}