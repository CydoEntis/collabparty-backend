using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDtoResponse> GetUserDetails(string userId);
    Task<UpdateUserResponseDto> UpdateUserDetails(string userId, UpdateUserRequestDto dto);
    Task AddGold(string userId, int amount);
    Task AddExperience(string userId, int amount);
}