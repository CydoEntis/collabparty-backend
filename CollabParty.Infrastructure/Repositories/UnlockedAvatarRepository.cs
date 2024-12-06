using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class UnlockedAvatarRepository : BaseRepository<Domain.Entities.UnlockedAvatar>, IUnlockedAvatarRepository
{
    private readonly AppDbContext _db;

    public UnlockedAvatarRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task AddRangeAsync(IEnumerable<Domain.Entities.UnlockedAvatar> userAvatars)
    {
        await _db.UnlockedAvatars.AddRangeAsync(userAvatars);
        await _db.SaveChangesAsync();
    }

    public async Task<Domain.Entities.UnlockedAvatar> UpdateAsync(Domain.Entities.UnlockedAvatar entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.UnlockedAvatars.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}