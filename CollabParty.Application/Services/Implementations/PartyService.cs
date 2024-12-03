// using CollabParty.Application.Common.Dtos.Avatar;
// using CollabParty.Application.Common.Dtos.Party;
// using CollabParty.Application.Common.Mappings;
// using CollabParty.Application.Common.Models;
// using CollabParty.Application.Services.Interfaces;
// using CollabParty.Domain.Entities;
// using CollabParty.Domain.Enums;
// using CollabParty.Domain.Interfaces;
// using Microsoft.Extensions.Logging;
//
// namespace CollabParty.Application.Services.Implementations;
//
// public class PartyService : IPartyService
// {
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly ILogger _logger;
//     private readonly IUserPartyService _userPartyService;
//
//
//     public PartyService(IUnitOfWork unitOfWork, IUserPartyService userPartyService, ILogger<Party> logger)
//     {
//         _unitOfWork = unitOfWork;
//         _userPartyService = userPartyService;
//         _logger = logger;
//     }
//
//     public async Task<Result<PartyDto>> CreateParty(string userId, CreatePartyDto dto)
//     {
//         try
//         {
//             var newParty = PartyMapper.FromCreatePartyDto(dto);
//             Party createdParty = await _unitOfWork.Party.CreateAsync(newParty);
//
//             await _userPartyService.AssignUserAndRole(userId, createdParty.Id, UserRole.Leader);
//
//             var foundParty = await _unitOfWork.Party.GetAsync(p => p.Id == newParty.Id,
//                 includeProperties: "User.UserAvatars.Avatar");
//
//             var partyDto = PartyMapper.ToPartyDto(foundParty);
//             return Result<PartyDto>.Success(partyDto);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to assign user to party.");
//             return Result<PartyDto>.Failure("An error occurred while creating the party.");
//         }
//     }
//     
//     // Todo: Add a method to Update A Party
//     
//     public async Task<Result<PartyDto>> UpdateParty(string userId, UpdatePartyDto dto)
//     {
//         try
//         {
//             var foundParty = await _unitOfWork.Party.GetAsync(
//                 p => p.Id == dto.PartyId,
//                 includeProperties: "UserParties.User.UserAvatars.Avatar");
//
//             if (foundParty == null)
//                 return Result<PartyDto>.Failure($"No party found with ID {dto.PartyId}");
//
//             var userParty = foundParty.UserParties
//                 .FirstOrDefault(up => up.UserId == userId && up.Role == UserRole.Leader);
//
//             if (userParty == null)
//                 return Result<PartyDto>.Failure("Only a party leader can update the party");
//
//             foundParty.PartyName = dto.PartyName;
//             foundParty.Description = dto.Description;
//             foundParty.UpdatedAt = DateTime.UtcNow;
//
//             var updatedParty = await _unitOfWork.Party.UpdateAsync(foundParty);
//
//             var partyDto = PartyMapper.ToPartyDto(updatedParty);
//             return Result<PartyDto>.Success(partyDto);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Failed to update party.");
//             return Result<PartyDto>.Failure("An error occurred while updating the party.");
//         }
//     }
//
//     
//     // Todo Add a method to Delete a party. - Deleting should also delete all the records in the UserParty table for that Party.
// }