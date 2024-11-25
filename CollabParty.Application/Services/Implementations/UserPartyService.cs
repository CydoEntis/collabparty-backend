using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class UserPartyService : IUserPartyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public UserPartyService(IUnitOfWork unitOfWork, ILogger<UserParty> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // public async Task<Result<UserParty>> AssignUserAndRole(string userId, int partyId, UserRole role)
    // {
    //     if (string.IsNullOrEmpty(userId))
    //         return Result<UserParty>.Failure("User Id is required.");
    //
    //     if (partyId <= 0)
    //         return Result<UserParty>.Failure("Party Id is required.");
    //
    //     try
    //     {
    //         UserParty newUserParty = new UserParty
    //         {
    //             PartyId = partyId,
    //             UserId = userId,
    //             Role = role
    //         };
    //
    //         var createdUserParty = await _unitOfWork.UserParty.CreateAsync(newUserParty);
    //         return Result<UserParty>.Success(createdUserParty);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to assign user to party.");
    //         return Result<UserParty>.Failure("An error occurred while assigning the user to the party.");
    //     }
    // }
    // public async Task<Result<int>> GetAllPartyMembers(int partyId)
    // {
    //     try
    //     {
    //         var partyMembers = await _unitOfWork.UserParty.GetAllAsync(ua => ua.PartyId == partyId, includeProperties: "User,User.UserAvatar");
    //         return partyMembers;
    //     }
    //     catch (Exception ex)
    //     {
    //         
    //     }
    // }
}