using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IAvatarService
{
    Task<Result<List<LockedAvatarDto>>> GetLockedAvatars(string userId);
}