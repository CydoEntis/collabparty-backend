using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class UnlockedAvatarService : IUnlockedAvatarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UnlockedAvatarService> _logger;

    public UnlockedAvatarService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UnlockedAvatarService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<LockedAvatarDto>>> GetUnlockableAvatars(string userId)
    {
        try
        {
            // Fetch all avatars, whether unlocked or not
            var avatars = await _unitOfWork.Avatar.GetAllAsync();

            if (!avatars.Any())
                return Result<List<LockedAvatarDto>>.Failure("No avatars found");

            var unlockedAvatars = await _unitOfWork.UnlockedAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

            var unlockedAvatarsLookup = unlockedAvatars.ToDictionary(ua => ua.AvatarId, ua => ua.IsUnlocked);

            var avatarDtos = avatars.Select(avatar => new LockedAvatarDto
            {
                Id = avatar.Id,
                Name = avatar.Name,
                DisplayName = avatar.DisplayName,
                ImageUrl = avatar.ImageUrl,
                Tier = avatar.Tier,
                UnlockLevel = avatar.UnlockLevel,
                UnlockCost = avatar.UnlockCost,
                IsUnlocked = unlockedAvatarsLookup.ContainsKey(avatar.Id) && unlockedAvatarsLookup[avatar.Id],
            }).ToList();

            return Result<List<LockedAvatarDto>>.Success(avatarDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving avatars");
            return Result<List<LockedAvatarDto>>.Failure(
                "An error occurred while retrieving avatars");
        }
    }

    public async Task<Result<AvatarResponseDto>> SetActiveAvatar(string userId, ActiveAvatarRequestDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<AvatarResponseDto>.Failure("User ID is required");

            var foundUserAvatar =
                await _unitOfWork.UnlockedAvatar.GetAsync(ua => ua.UserId == userId && ua.AvatarId == dto.AvatarId,
                    includeProperties: "Avatar");

            var currentActiveAvatar =
                await _unitOfWork.UnlockedAvatar.GetAsync(ua => ua.UserId == userId && ua.IsActive);

            if (currentActiveAvatar != null)
            {
                currentActiveAvatar.IsActive = false;
                await _unitOfWork.UnlockedAvatar.UpdateAsync(currentActiveAvatar);
            }

            if (foundUserAvatar == null)
                return Result<AvatarResponseDto>.Failure("User avatar does not exist");

            foundUserAvatar.IsActive = true;

            await _unitOfWork.UnlockedAvatar
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
                await _unitOfWork.UnlockedAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

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

    public async Task UnlockStarterAvatars(ApplicationUser user)
    {
        var starterAvatars = await _unitOfWork.Avatar.GetAllAsync(a => a.Tier == 0);

        var unlockedAvatars = starterAvatars.Select(avatar => new UnlockedAvatar
        {
            UserId = user.Id,
            AvatarId = avatar.Id,
            UnlockedAt = DateTime.UtcNow,
            IsActive = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        await _unitOfWork.UnlockedAvatar.AddRangeAsync(unlockedAvatars);
        await _unitOfWork.SaveAsync();
    }

    public async Task SetNewUserAvatar(string userId, int selectedAvatarId)
    {
        var activeAvatar =
            await _unitOfWork.UnlockedAvatar.GetAsync(ua => ua.UserId == userId && ua.AvatarId == selectedAvatarId);
        if (activeAvatar != null)
        {
            activeAvatar.IsActive = true;
            await _unitOfWork.UnlockedAvatar.UpdateAsync(activeAvatar);
            await _unitOfWork.SaveAsync();
        }
    }
}