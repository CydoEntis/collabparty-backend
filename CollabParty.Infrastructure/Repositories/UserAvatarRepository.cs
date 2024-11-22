using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class UserAvatarRepository : BaseRepository<UserAvatar>, IUserAvatarRepository
{
    private readonly AppDbContext _db;

    public UserAvatarRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<UserAvatar> UpdateAsync(UserAvatar entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.UserAvatars.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}