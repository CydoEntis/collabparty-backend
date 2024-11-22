using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IUserQuestRepository : IBaseRepository<UserQuest>
{
    Task<UserQuest> UpdateAsync(UserQuest entity);
}