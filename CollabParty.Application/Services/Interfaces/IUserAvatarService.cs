using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IUserAvatarService
{
    Task<Result<List<AvatarDto>>> GetAllUnlockedAvatars(string userId);
}