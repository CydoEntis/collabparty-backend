using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Interfaces;

public interface IPartyMemberRepository : IBaseRepository<PartyMember>
{
    Task<PaginatedResult<PartyMember>> GetPaginatedAsync(QueryParams<PartyMember>? queryParams);

    Task<List<PartyMember>> GetMostRecentPartiesForUserAsync(
        string userId,
        string includeProperties = "");

    Task<PartyMember> UpdateAsync(PartyMember entity);
    Task UpdateUsersAsync(List<PartyMember> userParties);
    Task RemoveUsersAsync(List<PartyMember> userParties);
}