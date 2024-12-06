using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class UserAvatarService : IUserAvatarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UserAvatarService> _logger;

    public UserAvatarService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserAvatarService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<AvatarResponseDto>>> GetAllUnlockedAvatars(string userId)
    {
        try
        {
            var unlockedAvatars =
                await _unitOfWork.UserAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

            if (!unlockedAvatars.Any())
                return Result<List<AvatarResponseDto>>.Failure("No avatars found");

            var avatars = _mapper.Map<List<AvatarResponseDto>>(unlockedAvatars);

            return Result<List<AvatarResponseDto>>.Success(avatars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party member roles");
            return Result<List<AvatarResponseDto>>.Failure("An error occured while getting all unlocked avatars");
        }
    }

    public async Task<Result<Dictionary<string, List<LockedAvatarDto>>>> GetAllAvatars(string userId)
    {
        try
        {
            // Fetch unlocked avatars for the user
            var unlockedAvatars =
                await _unitOfWork.UserAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

            if (!unlockedAvatars.Any())
                return Result<Dictionary<string, List<LockedAvatarDto>>>.Failure("No avatars found");

            // Map entities to DTOs
            var avatarDtos = _mapper.Map<List<LockedAvatarDto>>(unlockedAvatars);

            // Group avatars by Tier and rename keys
            var groupedByTier = avatarDtos
                .GroupBy(a => a.Tier) // Group by Tier
                .ToDictionary(
                    group => $"tier-{group.Key}", // Convert numerical key to string "tier-{number}"
                    group => group.ToList()
                );

            return Result<Dictionary<string, List<LockedAvatarDto>>>.Success(groupedByTier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while grouping avatars by tier");
            return Result<Dictionary<string, List<LockedAvatarDto>>>.Failure(
                "An error occurred while retrieving avatars");
        }
    }
}