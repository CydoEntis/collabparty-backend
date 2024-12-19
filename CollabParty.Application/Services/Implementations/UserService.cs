using AutoMapper;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Interfaces;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result<UserDtoResponse>> GetUserDetails(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<UserDtoResponse>.Failure("User ID is required");

            var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId,
                includeProperties: "UnlockedAvatars,UnlockedAvatars.Avatar");

            if (foundUser == null)
                return Result<UserDtoResponse>.Failure("User does not exist");

            var updatedUserDto = _mapper.Map<UserDtoResponse>(foundUser);

            return Result<UserDtoResponse>.Success(updatedUserDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            return Result<UserDtoResponse>.Failure("An error occurred while updating the user");
        }
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

    public async Task<Result> AddGold(string userId, int amount)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("User ID is required");

            var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);
            if (user == null)
                return Result.Failure("User does not exist");

            user.Gold += amount;
            await _unitOfWork.User.UpdateAsync(user);

            return Result.Success($"Successfully added {amount} gold to user with ID {userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to add gold to user with ID {userId}");
            return Result.Failure("An error occurred while adding gold");
        }
    }

    public async Task<Result> AddExperience(string userId, int amount)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result.Failure("User ID is required");

            var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);
            if (user == null)
                return Result.Failure("User does not exist");

            user.CurrentExp += amount;

            // Check for level-up
            var leveledUp = false;
            while (user.CurrentExp >= GetExperienceThreshold(user.CurrentLevel))
            {
                user.CurrentExp -= GetExperienceThreshold(user.CurrentLevel);
                user.CurrentExp++;
                leveledUp = true;
            }

            await _unitOfWork.User.UpdateAsync(user);

            var message = leveledUp
                ? $"User with ID {userId} leveled up to level {user.CurrentLevel}"
                : $"Successfully added {amount} experience to user with ID {userId}";

            return Result.Success(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to add experience to user with ID {userId}");
            return Result.Failure("An error occurred while adding experience");
        }
    }

    private int GetExperienceThreshold(int level)
    {
        return 100 + (level * 50);
    }
}