using CollabParty.Application.Interfaces;

namespace CollabParty.Domain.Interfaces;

public interface IUnitOfWork
{
    IAvatarRepository Avatar { get; }
    IPartyRepository Party { get; }
    IPartyMemberRepository PartyMember { get; }
    IQuestRepository Quest { get; }
    IQuestAssignmentRepository QuestAssignment { get; }
    IQuestCommentRepository QuestComment { get; }
    IQuestFileRepository QuestFile { get; }
    IQuestStepRepository QuestStep { get; }
    ISessionRepository Session { get; }
    IUnlockedAvatarRepository UnlockedAvatar { get; }
    IUserRepository User { get; }
    Task SaveAsync();
}