using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Mappings;
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

    public async Task<Result> AssignUserAndRole(string userId, int partyId, UserRole role)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");

        if (partyId <= 0)
            return Result.Failure("Party Id is required.");

        try
        {
            UserParty newUserParty = new UserParty
            {
                PartyId = partyId,
                UserId = userId,
                Role = role
            };

            await _unitOfWork.UserParty.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            return Result.Failure("An error occurred while assigning the user to the party.");
        }
    }

    public async Task<Result<List<PartyDto>>> GetAllPartiesForUser(string userId)
    {
        try
        {
            var parties = await _unitOfWork.UserParty.GetAllAsync(up => up.UserId == userId,
                includeProperties: "Party,User.UserAvatars.Avatar");

            var partyDtos = parties.Select(p => PartyMapper.ToPartyDto(p.Party)).ToList();

            return Result<List<PartyDto>>.Success(partyDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party.");
            return Result<List<PartyDto>>.Failure("An error occurred while creating party.");
        }
    }

    public async Task<Result<PartyDto>> GetParty(string userId, int partyId)
    {
        try
        {
            var foundParty = await _unitOfWork.UserParty.GetAsync(up => up.PartyId == partyId && up.UserId == userId,
                includeProperties: "Party,User.UserAvatars.Avatar");

            if (foundParty == null)
                return Result<PartyDto>.Failure($"No party with the {partyId} exists");

            var partyDto = PartyMapper.ToPartyDto(foundParty);
            return Result<PartyDto>.Success(partyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party with id");
            return Result<PartyDto>.Failure("An error occurred while fetching party.");
        }
    }

    public async Task<Result<PartyDto>> GetPartyMembers(string userId, int partyId)
    {
        try
        {
            var foundParty = await _unitOfWork.UserParty.GetAsync(up => up.PartyId == partyId && up.UserId == userId,
                includeProperties: "User.UserAvatars.Avatar");

            if (foundParty == null)
                return Result<PartyDto>.Failure($"No party with the {partyId} exists");

            var partyDto = PartyMapper.ToPartyDto(foundParty);
            return Result<PartyDto>.Success(partyDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party members");
            return Result<PartyDto>.Failure("An error occurred while fetching party members.");
        }
    }
}