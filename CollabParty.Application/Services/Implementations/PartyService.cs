using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class PartyService : IPartyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;
    private readonly IUserPartyService _userPartyService;


    public PartyService(IUnitOfWork unitOfWork, IUserPartyService userPartyService, ILogger<Party> logger)
    {
        _unitOfWork = unitOfWork;
        _userPartyService = userPartyService;
        _logger = logger;
    }

    // public async Task<Result<PartyDto>> CreateParty(string userId, CreatePartyDto dto)
    // {
    //     try
    //     {
    //         var newParty = _mapper.Map<Party>(dto);
    //         Party createdParty = await _unitOfWork.Party.CreateAsync(newParty);
    //
    //         var newUserParty = await _userPartyService.AssignUserAndRole(userId, createdParty.Id, UserRole.Leader);
    //
    //         var partyResult = await GetParty(newParty.Id);
    //
    //         var partyDto = _mapper.Map<PartyDto>(partyResult.Data);
    //         return Result<PartyDto>.Success(partyDto);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to assign user to party.");
    //         return Result<PartyDto>.Failure("An error occurred while creating the party.");
    //     }
    // }
    //
    //
    //
    // public async Task<Result<Party>> GetParty(int partyId)
    // {
    //     try
    //     {
    //         Party foundParty = await _unitOfWork.Party.GetAsync(p => p.Id == partyId, includeProperties: "UserParties.User.UserAvatars");
    //         
    //         if(foundParty == null)
    //             return Result<Party>.Failure($"No party with the {partyId} exists");
    //         
    //         return Result<Party>.Success(foundParty);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to assign user to party.");
    //         return Result<Party>.Failure("An error occurred while creating party.");
    //     }
    // }
}