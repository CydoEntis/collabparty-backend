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
    
    public async Task RemoveUsersAsync(List<UserParty> userParties)
    {
        if (userParties == null || !userParties.Any())
            throw new ArgumentException("The userParties list cannot be null or empty.", nameof(userParties));

        _db.UserParties.RemoveRange(userParties);
        await _db.SaveChangesAsync();
    }
}