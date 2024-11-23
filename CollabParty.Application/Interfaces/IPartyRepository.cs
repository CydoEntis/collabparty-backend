using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IPartyRepository : IBaseRepository<Party>
{
    Task<Party> UpdateAsync(Party entity);
}