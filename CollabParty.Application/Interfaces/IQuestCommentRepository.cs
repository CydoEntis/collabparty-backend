using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Interfaces;

public interface IQuestCommentRepository : IBaseRepository<QuestComment>
{
    Task<PaginatedResult<QuestComment>> GetPaginatedAsync(QueryParams<QuestComment>? queryParams);
    Task<QuestComment> UpdateAsync(QuestComment entity);
}