using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyMemberService
{
    Task<Result> AddPartyMember(string userId, int partyId);
    Task<Result> AddPartyLeader(string userId, int partyId);
}