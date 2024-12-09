using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Interfaces;

public interface IPartyRepository : IBaseRepository<Party>
{
    Task<PaginatedResult<Party>> GetPaginatedAsync(QueryParams<Party>? queryParams);

    Task<List<Party>> GetMostRecentPartiesForUserAsync(
        string userId,
        string includeProperties = "");

    Task<Party> UpdateAsync(Party entity);
    Task RemoveUsersAsync(List<Party> userParties);
}