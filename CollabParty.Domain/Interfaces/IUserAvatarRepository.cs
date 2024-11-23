using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IUserAvatarRepository : IBaseRepository<UserAvatar>
{
    Task AddRangeAsync(IEnumerable<UserAvatar> userAvatars);
    Task<UserAvatar> UpdateAsync(UserAvatar entity);
}