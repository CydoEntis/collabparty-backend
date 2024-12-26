using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyMemberService
{
    Task<Result> AddPartyMember(string userId, int partyId);
    Task<Result> AddPartyLeader(string userId, int partyId);

    Task<Result<List<PartyMemberResponseDto>>> GetPartyMembers(string userId, int partyId);

    Task<Result<int>> UpdatePartyMembers(int partyId, List<MemberUpdateDto> membersToUpdate);
    Task<Result> LeaveParty(string userId, int partyId);

    Task<Result<int>> ChangePartyLeader(int partyId, ChangePartyLeaderRequestDto dto);
}