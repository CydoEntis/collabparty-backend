using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestAssignmentService
{
    Task<Result> AssignPartyMembersToQuest(int questId, string[] partyMemberIds);
}