using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IUserQuestStepRepository : IBaseRepository<UserQuestStep>
{
    Task<UserQuestStep> UpdateAsync(UserQuestStep entity);
}