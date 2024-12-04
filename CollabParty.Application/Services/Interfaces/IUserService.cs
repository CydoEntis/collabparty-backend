using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IUserService
{
    Task<Result<UpdateUserResponseDto>> UpdateUserDetails(string userId, UpdateUserRequestDto dto);
}