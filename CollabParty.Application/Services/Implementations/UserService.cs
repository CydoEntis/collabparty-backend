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
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper,
        UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<UserDtoResponse> GetUserDetails(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User id is required.");

        var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId,
            includeProperties: "UnlockedAvatars,UnlockedAvatars.Avatar");

        if (EntityUtility.EntityIsNull(foundUser))
        {
            throw new NotFoundException("User not found.");
        }

        return _mapper.Map<UserDtoResponse>(foundUser);
    }


    public async Task<UpdateUserDetailsResponseDto> UpdateUserDetails(string userId, UpdateUserRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User ID is required.");

        var foundUser = await _unitOfWork.User.GetAsync(u => u.Id == userId);
        if (foundUser == null)
            throw new NotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            !dto.Email.Equals(foundUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExists = await _unitOfWork.User.GetAsync(u => u.Email == dto.Email && u.Id != userId);
            if (emailExists != null)
                throw new ValidationException(ErrorFields.Email, "Email is already taken.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Username) &&
            !dto.Username.Equals(foundUser.UserName, StringComparison.OrdinalIgnoreCase))
        {
            var usernameExists = await _unitOfWork.User.GetAsync(u => u.UserName == dto.Username && u.Id != userId);
            if (usernameExists != null)
                throw new ValidationException(ErrorFields.UserName, "Username is already taken.");
        }

        foundUser.Email = dto.Email ?? foundUser.Email;
        foundUser.NormalizedEmail = Normalize(dto.Email) ?? foundUser.NormalizedEmail;

        foundUser.UserName = dto.Username ?? foundUser.UserName;
        foundUser.NormalizedUserName = Normalize(dto.Username) ?? foundUser.NormalizedUserName;

        await _unitOfWork.User.UpdateAsync(foundUser);

        return _mapper.Map<UpdateUserDetailsResponseDto>(foundUser);
    }

    private string Normalize(string value)
    {
        return value?.ToUpperInvariant();
    }


    public async Task<AddGoldResponseDto> AddGold(string userId, int amount)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User id is required.");

        var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);

        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException("User not found.");

        user.Gold += amount;
        await _unitOfWork.User.UpdateAsync(user);

        return new AddGoldResponseDto { Message = "Gold awarded to user", Gold = user.Gold, UserId = user.Id };
    }

    public async Task<AddExpResponseDto> AddExperience(string userId, int amount)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User id is required.");

        var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);

        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException("User not found.");

        user.CurrentExp += amount;

        var leveledUp = false;
        while (user.CurrentExp >= GetExperienceThreshold(user.CurrentLevel))
        {
            user.CurrentExp -= GetExperienceThreshold(user.CurrentLevel);
            user.CurrentLevel++;
            leveledUp = true;
        }

        await _unitOfWork.User.UpdateAsync(user);

        var message = leveledUp
            ? $"User with id {userId} leveled up to level {user.CurrentLevel}"
            : $"Successfully added {amount} experience to user with ID {userId}";

        return new AddExpResponseDto { Message = message, NewCurrentExp = user.CurrentExp, UserId = user.Id };
    }

 public async Task<UserStatsResponseDto> GetUserStats(string userId)
{
    if (string.IsNullOrWhiteSpace(userId))
        throw new IsRequiredException("User ID is required.");

    // Fetch user details
    var user = await _unitOfWork.User.GetAsync(
        u => u.Id == userId,
        includeProperties: "UnlockedAvatars");

    if (EntityUtility.EntityIsNull(user))
        throw new NotFoundException("User not found.");

    var currentMonth = DateTime.UtcNow.Month;
    var currentYear = DateTime.UtcNow.Year;

    // Fetch quests assigned to the user
    var userQuests = await _unitOfWork.QuestAssignment.GetAllAsync(
        qa => qa.UserId == userId,
        includeProperties: "Quest");

    // Fetch parties the user is a member of
    var userParties = await _unitOfWork.Party.GetAllAsync(
        p => p.PartyMembers.Any(pm => pm.UserId == userId),
        includeProperties: "PartyMembers");

    // Fetch total avatar count
    var totalAvatars = await _unitOfWork.Avatar.CountAsync();

    // Count unlocked avatars for the user (assuming 'IsUnlocked' marks the avatar as unlocked)
    var unlockedAvatarCount = user.UnlockedAvatars?.Count(a => a.IsUnlocked) ?? 0;

    var assignedQuests = userQuests.Select(qa => qa.Quest).ToList();
    var completedQuests = assignedQuests.Where(q => q.IsCompleted && q.CompletedAt.Month == currentMonth && q.CompletedAt.Year == currentYear).ToList();

    // Group the completed quests by the day they were completed
    var completedQuestsByDay = completedQuests
        .GroupBy(q => q.CompletedAt.Date) // Group by date (removes the time part)
        .ToDictionary(
            group => group.Key,  // Key = Date of completion
            group => group.Count()  // Value = Number of quests completed on that date
        );

    // Calculate experience threshold
    var experienceThreshold = GetExperienceThreshold(user.CurrentLevel);
    var experienceToLevelUp = experienceThreshold - user.CurrentExp;

    return new UserStatsResponseDto
    {
        UserId = user.Id,
        CurrentLevel = user.CurrentLevel,
        CurrentExperience = user.CurrentExp,
        ExperienceThreshold = experienceThreshold,
        ExperienceToLevelUp = experienceToLevelUp,
        Gold = user.Gold,
        TotalQuests = assignedQuests.Count,
        CompletedQuests = completedQuests.Count,
        PartiesJoined = userParties?.Count() ?? 0,
        UnlockedAvatarCount = unlockedAvatarCount,
        TotalAvatarCount = totalAvatars,
        MonthlyCompletedQuestsByDay = completedQuestsByDay // Add the dictionary here
    };
}



    private int GetExperienceThreshold(int level)
    {
        return 100 + (level * 50);
    }
}