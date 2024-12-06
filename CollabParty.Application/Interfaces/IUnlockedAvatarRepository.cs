using CollabParty.Domain.Entities;

namespace CollabParty.Application.Interfaces;

public interface IUnlockedAvatarRepository : IBaseRepository<UnlockedAvatar>
{
    Task AddRangeAsync(IEnumerable<UnlockedAvatar> userAvatars);
    Task<UnlockedAvatar> UpdateAsync(UnlockedAvatar entity);
}