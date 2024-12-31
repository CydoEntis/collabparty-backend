using AutoMapper;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class PartyMemberService : IPartyMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PartyMember> _logger;
    private readonly IMapper _mapper;

    public PartyMemberService(IUnitOfWork unitOfWork, ILogger<PartyMember> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<AddPartyMemberResponseDto> AddPartyMember(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new NotFoundException("UserId cannot be null or empty.");

        if (partyId <= 0)
            throw new NotFoundException("PartyId cannot be null or empty.");

        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Member
            };

            var newPartyMember = await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return _mapper.Map<AddPartyMemberResponseDto>(newPartyMember);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add member to party");
            throw new ResourceCreationException("An error occurred while adding member to party.");
        }
    }

    public async Task<AddPartyMemberResponseDto> AddPartyLeader(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new NotFoundException("UserId cannot be null or empty.");

        if (partyId <= 0)
            throw new NotFoundException("PartyId cannot be null or empty.");

        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Leader
            };

            var newPartyMember = await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return _mapper.Map<AddPartyMemberResponseDto>(newPartyMember);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add member to party");
            throw new ResourceCreationException("An error occurred while adding member to party.");
        }
    }

    public async Task<List<PartyMemberResponseDto>> GetPartyMembers(string userId, int partyId)
    {
        try
        {
            var userPartyMember = await _unitOfWork.PartyMember.GetAsync(
                pm => pm.PartyId == partyId && pm.UserId == userId,
                includeProperties: "User,User.UnlockedAvatars,User.UnlockedAvatars.Avatar");


            if (EntityUtility.EntityIsNull(userPartyMember))
                throw new NotFoundException("Party member not found.");


            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(
                pm => pm.PartyId == partyId,
                includeProperties: "User,User.UnlockedAvatars,User.UnlockedAvatars.Avatar");


            if (!partyMembers.Any())
                throw new NotFoundException($"No members found for party with ID {partyId}.");

            return _mapper.Map<List<PartyMemberResponseDto>>(partyMembers);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party members for party {PartyId}.", partyId);
            throw new OperationException("Fetch Exception", "An error occured while fetching party members.");
        }
    }


    public async Task<UpdatePartyMemberResponseDto> ChangePartyLeader(int partyId, ChangePartyLeaderRequestDto dto)
    {
        if (partyId <= 0)
            throw new NotFoundException("PartyId cannot be null or empty.");


        if (string.IsNullOrEmpty(dto.CurrentLeaderId))
            throw new NotFoundException("CurrentLeaderId cannot be null or empty.");

        if (string.IsNullOrEmpty(dto.NewLeaderId))
            throw new NotFoundException("NewLeaderId cannot be null or empty.");

        try
        {
            var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId);
            if (EntityUtility.EntityIsNull(party))
                throw new NotFoundException("Party not found.");

            var currentLeader = await _unitOfWork.PartyMember.GetAsync(
                pm => pm.PartyId == partyId && pm.UserId == dto.CurrentLeaderId);

            if (currentLeader == null) throw new NotFoundException("Party leader not found.");

            if (!RoleUtility.IsLeader(currentLeader))
                throw new PermissionException("You do not have permission to change the leader.");

            var newLeader = await _unitOfWork.PartyMember.GetAsync(
                pm => pm.PartyId == partyId && pm.UserId == dto.NewLeaderId);

            if (EntityUtility.EntityIsNull(newLeader))
                throw new NotFoundException("New party leader not found.");

            currentLeader.Role = dto.NewRoleForPreviousLeader;
            newLeader.Role = UserRole.Leader;

            party.CreatedById = newLeader.UserId;

            await _unitOfWork.PartyMember.UpdateAsync(currentLeader);
            await _unitOfWork.PartyMember.UpdateAsync(newLeader);
            await _unitOfWork.Party.UpdateAsync(party);

            return new UpdatePartyMemberResponseDto() { Message = "Successfully changed leader.", PartyId = party.Id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change party leader.");
            throw new OperationException("Change Party Leader Exception",
                "An error occurred while changing the party leader.");
        }
    }


    public async Task<UpdatePartyMemberResponseDto> UpdatePartyMembers(int partyId,
        List<MemberUpdateDto> membersToUpdate)
    {
        try
        {
            var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId);
            if (EntityUtility.EntityIsNull(party))
                throw new NotFoundException("Party not found.");

            foreach (var member in membersToUpdate)
            {
                var partyMember = await _unitOfWork.PartyMember.GetAsync(
                    pm => pm.PartyId == partyId && pm.UserId == member.Id);


                if (EntityUtility.EntityIsNull(partyMember))
                    throw new NotFoundException($"Member with ID {member.Id} not found in the party.");


                if (member.Delete)
                {
                    await _unitOfWork.PartyMember.RemoveAsync(partyMember);
                }
                else
                {
                    partyMember.Role = (UserRole)member.Role;
                    await _unitOfWork.PartyMember.UpdateAsync(partyMember);
                }
            }

            await _unitOfWork.SaveAsync();

            return new UpdatePartyMemberResponseDto() { Message = "Party members updated", PartyId = party.Id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party members.");
            throw new OperationException("Update Party Members Exception",
                "An error occurred while updating party members.");
        }
    }


    public async Task<UpdatePartyMemberResponseDto> LeaveParty(string userId, int partyId)
    {
        try
        {
            var foundPartyMember =
                await _unitOfWork.PartyMember.GetAsync(pm => pm.PartyId == partyId && pm.UserId == userId);
            if (EntityUtility.EntityIsNull(foundPartyMember))
                throw new NotFoundException("Party member not found.");

            var questAssignments =
                await _unitOfWork.QuestAssignment.GetAllAsync(qa => qa.UserId == userId && qa.Quest.PartyId == partyId);
            foreach (var assignment in questAssignments)
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            await _unitOfWork.PartyMember.RemoveAsync(foundPartyMember);

            var remainingMembers =
                await _unitOfWork.PartyMember.GetAllAsync(pm => pm.PartyId == partyId);
            if (!remainingMembers.Any())
            {
                var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId);
                if (EntityUtility.EntityIsNull(party))
                    throw new NotFoundException("Party not found.");

                await _unitOfWork.Party.RemoveAsync(party);
            }

            return new UpdatePartyMemberResponseDto() { Message = "Party left successfully.", PartyId = partyId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to leave the party.");
            throw new OperationException("Leave Party Exception",
                "An error occurred while attempting to leave party.");
        }
    }


    // TODO: Implement these shits.
    
    // public async Task<Result> InvitePartyMember(string userId, int partyId, string inviteeEmail)
    // {
    //     try
    //     {
    //         var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId);
    //
    //         if (party == null)
    //             return Result.Failure("Party not found or you do not have permission to invite members.");
    //
    //         // Generate token (for simplicity, we'll just use a GUID as a token)
    //         var token = Guid.NewGuid().ToString();
    //         var tokenExpiration = DateTime.UtcNow.AddMinutes(15); // Token expires after 15 minutes
    //
    //         // Create an invite record in the database (store token and expiration date)
    //         var invite = new PartyInvite
    //         {
    //             PartyId = partyId,
    //             InviterUserId = userId,
    //             InviteeEmail = inviteeEmail,
    //             Token = token,
    //             ExpirationDate = tokenExpiration
    //         };
    //
    //         await _unitOfWork.PartyInvite.CreateAsync(invite);
    //
    //         // Send email with the invite link (simulating an email send here)
    //         var inviteLink = $"https://yourapp.com/party/invite/{token}";
    //         await _emailService.SendInviteEmailAsync(inviteeEmail, inviteLink);
    //
    //         return Result.Success();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to invite party member.");
    //         return Result.Failure("An error occurred while sending the invite.");
    //     }
    // }
    //
    // public async Task<Result> AcceptInvite(string token)
    // {
    //     try
    //     {
    //         // Fetch the invite by token
    //         var invite = await _unitOfWork.PartyInvite.GetAsync(i => i.Token == token);
    //
    //         if (invite == null || invite.ExpirationDate < DateTime.UtcNow)
    //             return Result.Failure("Invalid or expired invite token.");
    //
    //         // Check if user already a member of the party
    //         var isMember = await _unitOfWork.PartyMember.ExistsAsync(pm =>
    //             pm.PartyId == invite.PartyId && pm.UserId == invite.InviteeUserId);
    //         if (isMember)
    //             return Result.Failure("User is already a member of the party.");
    //
    //         // Add the invitee as a party member
    //         var partyMember = new PartyMember
    //         {
    //             PartyId = invite.PartyId,
    //             UserId = invite.InviteeUserId,
    //             Role = UserRole.Member,
    //             JoinedAt = DateTime.UtcNow
    //         };
    //         await _unitOfWork.PartyMember.CreateAsync(partyMember);
    //
    //         // Remove the invite to prevent re-use
    //         await _unitOfWork.PartyMemberInvite.RemoveAsync(invite);
    //
    //         return Result.Success();
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Failed to accept invite.");
    //         return Result.Failure("An error occurred while accepting the invite.");
    //     }
    // }
}