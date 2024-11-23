using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IQuestRepository : IBaseRepository<Quest>
{
    Task<Quest> UpdateAsync(Quest entity);
}