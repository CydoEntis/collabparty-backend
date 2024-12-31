using AutoMapper;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Errors;
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

    public async Task<List<LockedAvatarDto>> GetLockedAvatars(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new IsRequiredException("User id is required.");

            var lockedAvatars = await _unitOfWork.Avatar.GetAllAsync(a =>
                !a.UnlockedAvatars.Any(ua => ua.UserId == userId && ua.IsUnlocked && ua.UnlockedAt != null));

            if (lockedAvatars == null || !lockedAvatars.Any())
                throw new NotFoundException("All avatars are unlocked.");

            return _mapper.Map<List<LockedAvatarDto>>(lockedAvatars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get locked avatars for user {UserId}", userId);
            throw new FetchException("An error occured while getting all locked avatars.");
        }
    }
}