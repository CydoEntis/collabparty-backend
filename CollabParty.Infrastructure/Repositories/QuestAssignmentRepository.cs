using CollabParty.Application.Interfaces;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class QuestAssignmentRepository : BaseRepository<QuestAssignment>, IQuestAssignmentRepository
{
    private readonly AppDbContext _db;

    public QuestAssignmentRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }
}