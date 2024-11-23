using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IQuestStepRepository : IBaseRepository<QuestStep>
{
    Task<QuestStep> UpdateAsync(QuestStep entity);
}