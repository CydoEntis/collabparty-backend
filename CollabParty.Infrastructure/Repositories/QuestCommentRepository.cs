using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class QuestCommentRepository : BaseRepository<QuestComment>, IQuestCommentRepository
{
    private readonly AppDbContext _db;

    public QuestCommentRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<QuestComment> UpdateAsync(QuestComment entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _db.QuestComments.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}