using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyMemberService
{
    Task AddPartyMember(string userId, int partyId);
    Task AddPartyLeader(string userId, int partyId);

    Task<List<PartyMemberResponseDto>> GetPartyMembers(string userId, int partyId);

    Task<int> UpdatePartyMembers(int partyId, List<MemberUpdateDto> membersToUpdate);
    Task LeaveParty(string userId, int partyId);

    Task<int> ChangePartyLeader(int partyId, ChangePartyLeaderRequestDto dto);
}