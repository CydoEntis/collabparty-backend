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

    public async Task<Result<List<AvatarDto>>> GetAllUnlockedAvatars(string userId)
    {
        try
        {
            var unlockedAvatars =
                await _unitOfWork.UserAvatar.GetAllAsync(ua => ua.UserId == userId, includeProperties: "Avatar");

            if (!unlockedAvatars.Any())
                return Result<List<AvatarDto>>.Failure("No avatars found");

            var avatars = _mapper.Map<List<AvatarDto>>(unlockedAvatars);

            return Result<List<AvatarDto>>.Success(avatars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party member roles");
            return Result<List<AvatarDto>>.Failure("An error occured while getting all unlocked avatars");
        }
    }
}