using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IQuestStepRepository : IBaseRepository<QuestStep>
{
    Task<QuestStep> UpdateAsync(QuestStep entity);
}