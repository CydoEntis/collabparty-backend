using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using Questlog.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyService
{
    Task<Result<PartyDto>> CreateParty(string userId, CreatePartyDto dto);
    Task<Result<PaginatedResult<PartyDto>>> GetAllPartiesForUser(string userId, QueryParamsDto dto);
    Task<Result<List<PartyDto>>> GetRecentParties(string userId);
    Task<Result<PartyDto>> GetParty(string userId, int partyId);
}