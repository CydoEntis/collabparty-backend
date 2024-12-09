using AutoMapper;
using CollabParty.Application.Common.Models;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class PartyMemberService
{
    
    private readonly IUnitOfWork _unitOfWork;  
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    
    public async Task<Result> AddPartyMember(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");
     
        if (partyId <= 0)
            return Result.Failure("Party Id is required.");
     
        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Member
            };
     
            await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed add member to party");
            return Result.Failure("An error occurred while add member to party");
        }
    }
    
    public async Task<Result> AddPartyLeader(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");
     
        if (partyId <= 0)
            return Result.Failure("Party Id is required.");
     
        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Leader
            };
     
            await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party leader.");
            return Result.Failure("An error occurred while adding user as party leader.");
        }
    }
}