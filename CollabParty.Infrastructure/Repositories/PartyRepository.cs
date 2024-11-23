using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

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