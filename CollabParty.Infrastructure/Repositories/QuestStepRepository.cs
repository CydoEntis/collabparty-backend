using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class QuestStepRepository : BaseRepository<QuestStep>, IQuestStepRepository
{
    private readonly AppDbContext _db;

    public QuestStepRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<QuestStep> UpdateAsync(QuestStep entity)
    {
        entity.UpdatedAt = DateTime.Now;
        _db.QuestSteps.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}