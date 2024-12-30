using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyService
{
    Task<PartyResponseDto> CreateParty(string userId, CreatePartyRequestDto requestDto);
    Task<PaginatedResult<PartyResponseDto>> GetAllPartiesForUser(string userId, QueryParamsDto dto);
    Task<List<PartyResponseDto>> GetRecentParties(string userId);
    Task<PartyResponseDto> GetParty(string userId, int partyId);
    Task<UpdatePartyResponseDto> UpdateParty(string userId, int partyId, UpdatePartyRequestDto requestDto);
    Task<DeletePartyResponseDto> DeleteParty(string userId, int partyId);
}