using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IPartyRepository : IBaseRepository<Party>
{
    Task<Party> UpdateAsync(Party entity);
}