using CollabParty.Application.Common.Dtos.QuestAssignment;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestAssignmentService
{
    Task<AssignPartyMembersResponseDto> AssignPartyMembersToQuest(int questId, string[] partyMemberIds);
    Task<UpdateAssignedPartyMembersResponseDto> UpdateQuestAssignments(int questId, string[] updatedPartyMemberIds);
    Task<DeleteAssignedPartyMembersResponseDto> DeleteQuestAssignments(int questId, string[] partyMemberIdsToDelete);
}