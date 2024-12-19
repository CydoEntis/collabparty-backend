using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestStepService
{
    Task<Result> CreateQuestSteps(int questId, string[] steps);
    Task<Result<int>> UpdateStepStatus(QuestStepStatusDto dto);
}