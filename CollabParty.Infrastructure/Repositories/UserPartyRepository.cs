using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class UserPartyRepository : BaseRepository<UserParty>, IUserPartyRepository
{
    private readonly AppDbContext _db;

    public UserPartyRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<UserParty> UpdateAsync(UserParty entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.UserParties.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}