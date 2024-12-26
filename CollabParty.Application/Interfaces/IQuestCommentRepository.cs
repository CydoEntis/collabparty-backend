using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IQuestCommentRepository : IBaseRepository<QuestComment>
{
    Task<QuestComment> UpdateAsync(QuestComment entity);
}