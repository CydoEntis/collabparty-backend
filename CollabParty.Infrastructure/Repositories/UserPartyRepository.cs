using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

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