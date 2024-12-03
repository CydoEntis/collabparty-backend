using CollabParty.Application.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class QuestFileRepository : BaseRepository<QuestFile>, IQuestFileRepository
{
    private readonly AppDbContext _db;

    public QuestFileRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}