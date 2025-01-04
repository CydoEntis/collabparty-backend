using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
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

    public async Task<List<LockedAvatarDto>> GetUnlockableAvatars(string userId)
    {
        var avatars = await _unitOfWork.Avatar.GetAllAsync();

        if (!avatars.Any())
            throw new NotFoundException("No avatars found");

        var unlockedAvatars =
            await _unitOfWork.UnlockedAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

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

        return avatarDtos;
    }

    public async Task<AvatarResponseDto> SetActiveAvatar(string userId, SelectedAvatarRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User id is required.");

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

        if (EntityUtility.EntityIsNull(foundUserAvatar))
            throw new NotFoundException("No avatar found");

        foundUserAvatar.IsActive = true;

        await _unitOfWork.UnlockedAvatar
            .UpdateAsync(foundUserAvatar);


        return _mapper.Map<AvatarResponseDto>(foundUserAvatar);
    }

    public async Task<List<AvatarResponseDto>> GetUnlockedAvatars(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User id is required.");

        var unlockedAvatars =
            await _unitOfWork.UnlockedAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");


        if (!unlockedAvatars.Any())
            throw new NotFoundException("No unlocked avatars found.");


        return _mapper.Map<List<AvatarResponseDto>>(unlockedAvatars);
    }

    public async Task<AvatarResponseDto> UnlockAvatar(string userId, SelectedAvatarRequestDto requestDto)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new IsRequiredException("User id is required.");

        var user = await _unitOfWork.User.GetAsync(u => u.Id == userId);
        if (EntityUtility.EntityIsNull(user))
            throw new NotFoundException("No user found.");

        var userLevel = user.CurrentLevel;
        var userGold = user.Gold;

        var avatar = await _unitOfWork.Avatar.GetAsync(a => a.Id == requestDto.AvatarId);
        if (EntityUtility.EntityIsNull(avatar))
            throw new NotFoundException("No avatar found.");


        if (userLevel < avatar.UnlockLevel)
            throw new ConflictException("Level requirement not met to unlock avatar.");

        if (userGold < avatar.UnlockCost)
            throw new ConflictException("Not enough gold to unlock avatar.");

        var isAvatarAlreadyUnlocked = await _unitOfWork.UnlockedAvatar.GetAsync(
            ua => ua.AvatarId == requestDto.AvatarId && ua.UserId == userId);

        if (isAvatarAlreadyUnlocked != null)
            if (!EntityUtility.EntityIsNull(isAvatarAlreadyUnlocked))
                throw new ConflictException("Avatar already unlocked.");

        var currentAvatar = await _unitOfWork.UnlockedAvatar.GetAsync(ua => ua.UserId == userId && ua.IsActive);
        currentAvatar.IsActive = false;

        await _unitOfWork.UnlockedAvatar.UpdateAsync(currentAvatar);

        var newUnlockedAvatar = new UnlockedAvatar
        {
            UserId = user.Id,
            AvatarId = avatar.Id,
            UnlockedAt = DateTime.UtcNow,
            IsUnlocked = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.UnlockedAvatar.CreateAsync(newUnlockedAvatar);

        user.Gold -= avatar.UnlockCost;

        if (user.Gold < 0) user.Gold = 0;

        await _unitOfWork.SaveAsync();

        return _mapper.Map<AvatarResponseDto>(newUnlockedAvatar);
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