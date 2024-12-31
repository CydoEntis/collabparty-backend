using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Services.Interfaces;

public interface IUnlockedAvatarService
{
    Task<List<LockedAvatarDto>> GetUnlockableAvatars(string userId);
    Task<AvatarResponseDto> SetActiveAvatar(string userId, SelectedAvatarRequestDto dto);
    Task<List<AvatarResponseDto>> GetUnlockedAvatars(string userId);
    Task<AvatarResponseDto> UnlockAvatar(string userId, SelectedAvatarRequestDto requestDto);
    Task UnlockStarterAvatars(ApplicationUser user);
    Task SetNewUserAvatar(string userId, int selectedAvatarId);

    
}