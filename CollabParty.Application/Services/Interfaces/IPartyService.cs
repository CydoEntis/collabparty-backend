using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyService
{
    Task<PartyDto> CreateParty(string userId, CreatePartyDto dto);
    Task<PaginatedResult<PartyDto>> GetAllPartiesForUser(string userId, QueryParamsDto dto);
    Task<List<PartyDto>> GetRecentParties(string userId);
    Task<PartyDto> GetParty(string userId, int partyId);
    Task<int> UpdateParty(string userId, int partyId, UpdatePartyDto dto);
    Task<int> DeleteParty(string userId, int partyId);
}