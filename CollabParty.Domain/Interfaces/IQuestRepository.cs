using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IQuestRepository : IBaseRepository<Quest>
{
    Task<Quest> UpdateAsync(Quest entity);
}