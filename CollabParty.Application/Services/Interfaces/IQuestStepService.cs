using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IQuestStepService
{
    Task<Result> CreateQuestSteps(int questId, string[] steps);
}