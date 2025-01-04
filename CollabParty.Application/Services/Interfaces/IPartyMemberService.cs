using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;

namespace CollabParty.Application.Services.Interfaces;

public interface IPartyMemberService
{
    Task<AddPartyMemberResponseDto> AddPartyMember(string userId, int partyId);
    Task<AddPartyMemberResponseDto> AddPartyLeader(string userId, int partyId);

    Task<List<PartyMemberResponseDto>> GetPartyMembers(string userId, int partyId);

    Task<UpdatePartyMemberResponseDto> ChangePartyLeader(int partyId, ChangePartyLeaderRequestDto dto);

    Task<UpdatePartyMemberResponseDto> UpdatePartyMembers(int partyId,
        List<MemberUpdateDto> membersToUpdate);

    Task<UpdatePartyMemberResponseDto> LeaveParty(string userId, int partyId);

    Task<InvitePartyMemberResponseDto> InvitePartyMember(string userId, int partyId, string inviteeEmail);
    Task<AcceptInviteResponseDto> AcceptInvite(string userId, string token);
}