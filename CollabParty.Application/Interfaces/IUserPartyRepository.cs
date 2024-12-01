using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Interfaces;

public interface IUserPartyRepository : IBaseRepository<UserParty>
{
    Task<PaginatedResult<UserParty>> GetPaginatedAsync(QueryParams<UserParty>? queryParams);

    Task<List<UserParty>> GetMostRecentPartiesForUserAsync(
        string userId,
        string includeProperties = "");
    Task<UserParty> UpdateAsync(UserParty entity);
    Task UpdateUsersAsync(List<UserParty> userParties);
    Task RemoveUsersAsync(List<UserParty> userParties);
}