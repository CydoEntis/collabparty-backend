using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class AvatarService : IAvatarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<AvatarService> _logger;

    public AvatarService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AvatarService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<LockedAvatarDto>>> GetLockedAvatars(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<List<LockedAvatarDto>>.Failure("User ID is required");

            var lockedAvatars = await _unitOfWork.Avatar.GetAllAsync(a =>
                !a.UserAvatars.Any(ua => ua.UserId == userId && ua.IsUnlocked && ua.UnlockedAt != null));

            if (lockedAvatars == null || !lockedAvatars.Any())
                return Result<List<LockedAvatarDto>>.Success(new List<LockedAvatarDto>());

            // Map to LockedAvatarDto
            var lockedAvatarDtos = _mapper.Map<List<LockedAvatarDto>>(lockedAvatars);

            return Result<List<LockedAvatarDto>>.Success(lockedAvatarDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get locked avatars for user {UserId}", userId);
            return Result<List<LockedAvatarDto>>.Failure("An error occurred while retrieving locked avatars");
        }
    }

}