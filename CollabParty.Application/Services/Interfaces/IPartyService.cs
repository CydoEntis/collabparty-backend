using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyService
{
    Task<Result<PartyDto>> CreateParty(string userId, CreatePartyDto dto);
}