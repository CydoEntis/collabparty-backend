using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Interfaces;

public interface IQuestRepository : IBaseRepository<Quest>
{
    Task<Quest> UpdateAsync(Quest entity);
    Task<PaginatedResult<Quest>> GetPaginatedAsync(QueryParams<Quest>? queryParams);
}