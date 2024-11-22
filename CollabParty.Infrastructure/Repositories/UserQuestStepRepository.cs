using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;
using Questlog.Infrastructure.Repositories;

namespace CollabParty.Infrastructure.Repositories;

public class UserQuestStepRepository : BaseRepository<UserQuestStep>, IUserQuestStepRepository
{
    private readonly AppDbContext _db;

    public UserQuestStepRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
    
    public async Task<UserQuestStep> UpdateAsync(UserQuestStep entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.UserQuestSteps.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}