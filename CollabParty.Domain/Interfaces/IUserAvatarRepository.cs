using CollabParty.Domain.Entities;

namespace CollabParty.Domain.Interfaces;

public interface IUserAvatarRepository : IBaseRepository<UserAvatar>
{
    Task<UserAvatar> UpdateAsync(UserAvatar entity);
}