using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class PartyRepository : BaseRepository<Party>, IPartyRepository
{
    private readonly AppDbContext _db;

    public PartyRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<Party> UpdateAsync(Party entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.Parties.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}