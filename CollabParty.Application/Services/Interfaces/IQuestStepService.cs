using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestStepService
{
    Task<CreateQuestStepResponseDto> CreateQuestSteps(int questId, string[] steps);
    Task<UpdateQuestStepResponseDto> UpdateStepStatus(QuestStepStatusDto dto);
    Task<UpdateQuestStepResponseDto> UpdateQuestSteps(int questId, List<UpdateQuestStepDto> updatedSteps);
    Task<DeleteQuestStepResponseDto> RemoveQuestSteps(int questId, List<int> stepIdsToRemove);
}