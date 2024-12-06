using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Services.Interfaces;

public interface IUnlockedAvatarService
{
    Task<Result<List<AvatarResponseDto>>> GetUnlockedAvatars(string userId);
    Task<Result<Dictionary<string, List<LockedAvatarDto>>>> GetUnlockableAvatars(string userId);
    Task<Result<AvatarResponseDto>> SetActiveAvatar(string userId, ActiveAvatarRequestDto dto);

    Task UnlockStarterAvatars(ApplicationUser user);

    Task SetNewUserAvatar(string userId, int selectedAvatarId);
}