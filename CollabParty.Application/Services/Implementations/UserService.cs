using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<UpdateUserResponseDto>> UpdateUserDetails(string userId, UpdateUserRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<UpdateUserResponseDto>.Failure("User ID is required");

            var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId);

            if (foundUser == null)
                return Result<UpdateUserResponseDto>.Failure("User does not exist");

            var emailTaken = await _unitOfWork.User.GetAsync(u => u.Email == dto.Email && u.Id != userId);
            if (emailTaken != null)
                return Result<UpdateUserResponseDto>.Failure("email", new[] { "Email is already taken" });

            var usernameTaken =
                await _unitOfWork.User.GetAsync(u => u.UserName == dto.Username && u.Id != userId);
            if (usernameTaken != null)
                return Result<UpdateUserResponseDto>.Failure("username", new[] { "Username is already taken" });

            foundUser.Email = dto.Email;
            foundUser.UserName = dto.Username;

            await _unitOfWork.User.UpdateAsync(foundUser);

            var updatedUserDto = _mapper.Map<UpdateUserResponseDto>(foundUser);

            return Result<UpdateUserResponseDto>.Success(updatedUserDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            return Result<UpdateUserResponseDto>.Failure("An error occurred while updating the user");
        }
    }


    public async Task<Result<AvatarResponseDto>> UpdateUserAvatar(string userId, UpdateUserAvatarRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<AvatarResponseDto>.Failure("User ID is required");

            var foundUserAvatar =
                await _unitOfWork.UserAvatar.GetAsync(ua => ua.UserId == userId && ua.AvatarId == dto.AvatarId, includeProperties: "Avatar");

            var currentActiveAvatar = await _unitOfWork.UserAvatar.GetAsync(ua => ua.UserId == userId && ua.IsActive);

            if (currentActiveAvatar != null)
            {
                currentActiveAvatar.IsActive = false;
                await _unitOfWork.UserAvatar.UpdateAsync(currentActiveAvatar);
            }

            if (foundUserAvatar == null)
                return Result<AvatarResponseDto>.Failure("User avatar does not exist");

            foundUserAvatar.IsActive = true;

            await _unitOfWork.UserAvatar
                .UpdateAsync(foundUserAvatar);


            var updatedUserAvatarDto = _mapper.Map<AvatarResponseDto>(foundUserAvatar);

            return Result<AvatarResponseDto>.Success(updatedUserAvatarDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            return Result<AvatarResponseDto>.Failure("An error occurred while updating the user");
        }
    }

    public async Task<Result<List<AvatarResponseDto>>> GetUnlockedAvatars(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<List<AvatarResponseDto>>.Failure("User ID is required");

            var unlockedAvatars =
                await _unitOfWork.UserAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

            if (!unlockedAvatars.Any())
                return Result<List<AvatarResponseDto>>.Failure("User does not have any unlocked avatars");


            var avatarDtoList = _mapper.Map<List<AvatarResponseDto>>(unlockedAvatars);

            return Result<List<AvatarResponseDto>>.Success(avatarDtoList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            return Result<List<AvatarResponseDto>>.Failure("An error occurred while updating the user");
        }
    }
}