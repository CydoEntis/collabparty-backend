using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserDtoResponse> GetUserDetails(string userId);
    Task<UpdateUserDetailsResponseDto> UpdateUserDetails(string userId, UpdateUserRequestDto dto);
    Task<AddGoldResponseDto> AddGold(string userId, int amount);
    Task<AddExpResponseDto> AddExperience(string userId, int amount);
    Task<UserStatsResponseDto> GetUserStats(string userId);
    Task<Dictionary<DateTime, int>> GetMonthlyCompletedQuestsByDay(string userId, int currentMonth, int currentYear);
}