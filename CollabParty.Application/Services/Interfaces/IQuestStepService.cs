using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestStepService
{
    Task CreateQuestSteps(int questId, string[] steps);
    Task UpdateStepStatus(QuestStepStatusDto dto);
    Task UpdateQuestSteps(int questId, List<UpdateQuestStepDto> updatedSteps);
    Task RemoveQuestSteps(int questId, List<int> stepIdsToRemove);
}