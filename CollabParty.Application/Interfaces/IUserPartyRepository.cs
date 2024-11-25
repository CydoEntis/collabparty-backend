using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IUserPartyRepository : IBaseRepository<UserParty>
{
    Task<UserParty> UpdateAsync(UserParty entity);
    Task RemoveUsersAsync(List<UserParty> userParties);
}