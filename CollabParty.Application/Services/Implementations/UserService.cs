using AutoMapper;
using CollabParty.Application.Common.Constants;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
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

    public async Task<UserDtoResponse> GetUserDetails(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new IsRequiredException("User id is required");

            var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId,
                includeProperties: "UnlockedAvatars,UnlockedAvatars.Avatar");

            if (EntityUtility.EntityIsNull(foundUser))
                throw new NotFoundException("User not found.");

            return _mapper.Map<UserDtoResponse>(foundUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            throw new ResourceModificationException("An error occured while updating the user.");
        }
    }

    public async Task<UpdateUserDetailsResponseDto> UpdateUserDetails(string userId, UpdateUserRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new IsRequiredException("User id is required");

            var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId);

            if (EntityUtility.EntityIsNull(foundUser))
                throw new NotFoundException("User not found.");

            var user = await _unitOfWork.User.GetAsync(u => u.Email == dto.Email && u.Id != userId);
            if (!EntityUtility.EntityIsNull(user.Email))
                throw new ValidationException(ErrorFields.Email, "Email is already taken");


            if (!EntityUtility.EntityIsNull(user.UserName))
                throw new ValidationException(ErrorFields.UserName, "Username is already taken");

            foundUser.Email = dto.Email;
            foundUser.UserName = dto.Username;

            await _unitOfWork.User.UpdateAsync(foundUser);

            return _mapper.Map<UpdateUserDetailsResponseDto>(foundUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user details");
            throw new ResourceModificationException("An error occured while updating the user.");
        }
    }

    public async Task<AddGoldResponseDto> AddGold(string userId, int amount)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new IsRequiredException("User id is required");

            var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);

            if (EntityUtility.EntityIsNull(user))
                throw new NotFoundException("User not found.");

            user.Gold += amount;
            await _unitOfWork.User.UpdateAsync(user);

            return new AddGoldResponseDto { Message = "Gold awarded to user", Gold = user.Gold, UserId = user.Id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to add gold to user with ID {userId}");
            throw new ResourceModificationException("An error occured while awarding the user gold.");
        }
    }

    public async Task<AddExpResponseDto> AddExperience(string userId, int amount)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new IsRequiredException("User id is required");

            var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);

            if (EntityUtility.EntityIsNull(user))
                throw new NotFoundException("User not found.");

            user.CurrentExp += amount;

            var leveledUp = false;
            while (user.CurrentExp >= GetExperienceThreshold(user.CurrentLevel))
            {
                user.CurrentExp -= GetExperienceThreshold(user.CurrentLevel);
                user.CurrentExp++;
                leveledUp = true;
            }

            await _unitOfWork.User.UpdateAsync(user);

            var message = leveledUp
                ? $"User with id {userId} leveled up to level {user.CurrentLevel}"
                : $"Successfully added {amount} experience to user with ID {userId}";

            return new AddExpResponseDto { Message = message, NewCurrentExp = user.CurrentExp, UserId = user.Id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to add experience to user with ID {userId}");
            throw new ResourceModificationException("An error occured while awarding experience.");
        }
    }

    private int GetExperienceThreshold(int level)
    {
        return 100 + (level * 50);
    }
}