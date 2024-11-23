using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IUserQuestRepository : IBaseRepository<UserQuest>
{
    Task<UserQuest> UpdateAsync(UserQuest entity);
}