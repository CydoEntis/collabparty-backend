using CollabParty.Application.Interfaces;

namespace CollabParty.Domain.Interfaces;

public interface IUnitOfWork
{
    IAvatarRepository Avatar { get; }
    IPartyRepository Party { get; }
    IQuestRepository Quest { get; }
    IQuestStepRepository QuestStep { get; }
    ISessionRepository Session { get; }
    IUserAvatarRepository UserAvatar { get; }
    IUserPartyRepository UserParty { get; }
    IUserQuestRepository UserQuest { get; }
    IUserRepository User { get; }
    Task SaveAsync();
}