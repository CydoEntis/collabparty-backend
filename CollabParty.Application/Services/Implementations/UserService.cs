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
using CollabParty.Domain.Enums;
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

        var user = await _unitOfWork.User.GetAsync(
            u => u.Id == userId,
            includeProperties: "UnlockedAvatars.Avatar");

        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException("User not found.");

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var currentDate = DateTime.UtcNow;

        var allAssignedQuests = await _unitOfWork.QuestAssignment.GetAllAsync(
            qa => qa.UserId == userId,
            includeProperties: "Quest");

        var assignedQuestList = allAssignedQuests.Select(qa => qa.Quest).ToList();

        var completedQuests = assignedQuestList
            .Where(q => q.IsCompleted &&
                        q.CompletedAt.HasValue &&
                        q.CompletedAt.Value.Month == currentMonth &&
                        q.CompletedAt.Value.Year == currentYear)
            .ToList();

        var overdueQuests = assignedQuestList
            .Where(q => !q.IsCompleted &&
                        q.DueDate < currentDate)
            .ToList();

        var userParties = await _unitOfWork.Party.GetAllAsync(
            p => p.PartyMembers.Any(pm => pm.UserId == userId),
            includeProperties: "PartyMembers");

        var totalAvatars = await _unitOfWork.Avatar.CountAsync();
        var unlockedAvatarCount = user.UnlockedAvatars?.Count(a => a.IsUnlocked) ?? 0;

        var lowQuests = assignedQuestList.Count(q => q.PriorityLevel == PriorityLevelOption.Low);
        var mediumQuests = assignedQuestList.Count(q => q.PriorityLevel == PriorityLevelOption.Medium);
        var highQuests = assignedQuestList.Count(q => q.PriorityLevel == PriorityLevelOption.High);
        var criticalQuests = assignedQuestList.Count(q => q.PriorityLevel == PriorityLevelOption.Critical);

        var currentAvatar = user.UnlockedAvatars
            .FirstOrDefault(ua => ua.IsActive)?.Avatar; 

        var currentAvatarDto = currentAvatar != null ? new AvatarResponseDto
        {
            Id = currentAvatar.Id,
            Name = currentAvatar.Name,
            ImageUrl = currentAvatar.ImageUrl,
            DisplayName = currentAvatar.DisplayName,
        } : null;
        
        return new UserStatsResponseDto
        {
            UserId = user.Id,
            Username = user.UserName, 
            CurrentAvatar = currentAvatarDto,
            CurrentLevel = user.CurrentLevel,
            CurrentExperience = user.CurrentExp,
            ExperienceToLevelUp = user.ExpToNextLevel,
            Gold = user.Gold,
            TotalQuests = assignedQuestList.Count,
            CompletedQuests = completedQuests.Count,
            PastDueQuests = overdueQuests.Count,
            PartiesJoined = userParties?.Count() ?? 0,
            UnlockedAvatarCount = unlockedAvatarCount,
            TotalAvatarCount = totalAvatars,
            LowQuests = lowQuests, 
            MediumQuests = mediumQuests, 
            HighQuests = highQuests, 
            CriticalQuests = criticalQuests 
        };
    }


    public async Task<Dictionary<int, int>> GetMonthlyCompletedQuestsByDay(string userId, int currentMonth,
        int currentYear)
    {
        var userQuests = await _unitOfWork.QuestAssignment.GetAllAsync(
            qa => qa.UserId == userId,
            includeProperties: "Quest");

        var assignedQuests = userQuests.Select(qa => qa.Quest).ToList();

        var completedQuests = assignedQuests
            .Where(q => q.IsCompleted && q.CompletedAt.HasValue &&
                        q.CompletedAt.Value.Month == currentMonth &&
                        q.CompletedAt.Value.Year == currentYear)
            .ToList();

        var completedQuestsByDay = completedQuests
            .Where(q => q.CompletedAt.HasValue)
            .GroupBy(q => q.CompletedAt.Value.Day)
            .OrderBy(group => group.Key)
            .ToDictionary(
                group => group.Key,
                group => group.Count()
            );

        return completedQuestsByDay;
    }


    private int GetExperienceThreshold(int level)
    {
        return 100 + (level * 50);
    }
}