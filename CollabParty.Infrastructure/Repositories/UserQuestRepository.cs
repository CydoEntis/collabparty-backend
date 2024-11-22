using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class UserQuestRepository : BaseRepository<UserQuest>, IUserQuestRepository
{
    private readonly AppDbContext _db;

    public UserQuestRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<UserQuest> UpdateAsync(UserQuest entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.UserQuests.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}