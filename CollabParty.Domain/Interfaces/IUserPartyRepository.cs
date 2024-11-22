using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IUserPartyRepository : IBaseRepository<UserParty>
{
    Task<UserParty> UpdateAsync(UserParty entity);
}