using CollabParty.Application.Interfaces;
using CollabParty.Domain.Interfaces;
using CollabParty.Infrastructure.Data;

namespace CollabParty.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public IAvatarRepository Avatar { get; private set; }
    public IPartyRepository Party { get; private set; }
    public IQuestRepository Quest { get; private set; }
    public IQuestStepRepository QuestStep { get; private set; }
    public ISessionRepository Session { get; private set; }
    public IUserAvatarRepository UserAvatar { get; private set; }
    public IUserPartyRepository UserParty { get; private set; }
    public IUserQuestRepository UserQuest { get; private set; }
    public IUserRepository User { get; private set; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;

        Avatar = new AvatarRepository(db);
        Party = new PartyRepository(db);

        Quest = new QuestRepository(db);
        QuestStep = new QuestStepRepository(db);
        Session = new SessionRepository(db);
        UserAvatar = new UserAvatarRepository(db);
        UserParty = new UserPartyRepository(db);
        UserQuest = new UserQuestRepository(db);
        User = new UserRepository(db);
    }


    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}