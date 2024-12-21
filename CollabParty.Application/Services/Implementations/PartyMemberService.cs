using AutoMapper;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
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

    public async Task<Result> AddPartyMember(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");

        if (partyId <= 0)
            return Result.Failure("Party Id is required.");

        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Member
            };

            await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed add member to party");
            return Result.Failure("An error occurred while add member to party");
        }
    }

    public async Task<Result> AddPartyLeader(string userId, int partyId)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure("User Id is required.");

        if (partyId <= 0)
            return Result.Failure("Party Id is required.");

        try
        {
            PartyMember newUserParty = new PartyMember
            {
                PartyId = partyId,
                UserId = userId,
                Role = UserRole.Leader
            };

            await _unitOfWork.PartyMember.CreateAsync(newUserParty);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to assign user to party leader.");
            return Result.Failure("An error occurred while adding user as party leader.");
        }
    }

    public async Task<Result<List<PartyMemberResponseDto>>> GetPartyMembers(string userId, int partyId)
    {
        try
        {
            var userPartyMember = await _unitOfWork.PartyMember.GetAsync(
                pm => pm.PartyId == partyId && pm.UserId == userId,
                includeProperties: "User,User.UnlockedAvatars,User.UnlockedAvatars.Avatar");


            if (userPartyMember == null)
            {
                return Result<List<PartyMemberResponseDto>>.Failure(
                    "You are not a member of this party or the party does not exist.");
            }


            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(
                pm => pm.PartyId == partyId,
                includeProperties: "User,User.UnlockedAvatars,User.UnlockedAvatars.Avatar");


            if (!partyMembers.Any())
            {
                return Result<List<PartyMemberResponseDto>>.Failure(
                    $"No members found for party with ID {partyId}.");
            }


            var partyMemberDtos = _mapper.Map<List<PartyMemberResponseDto>>(partyMembers);
            return Result<List<PartyMemberResponseDto>>.Success(partyMemberDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party members for party {PartyId}.", partyId);
            return Result<List<PartyMemberResponseDto>>.Failure("An error occurred while fetching party members.");
        }
    }


    public async Task<Result<List<PartyMemberResponseDto>>> RemovePartyMembers(string userId, int partyId,
        RemoverUserFromPartyDto dto)
    {
        try
        {
            var foundParty = await _unitOfWork.PartyMember.GetAsync(
                up => up.PartyId == partyId && up.UserId == userId,
                includeProperties: "PartyMembers");

            if (foundParty == null)
                return Result<List<PartyMemberResponseDto>>.Failure($"No party with the {dto.PartyId} exists");

            if (foundParty.Role == UserRole.Member)
                return Result<List<PartyMemberResponseDto>>.Failure(
                    "User must have a role of Leader or Captain to remove members");

            var usersToRemove = await _unitOfWork.PartyMember.GetAllAsync(
                up => up.PartyId == dto.PartyId && dto.UserIds.Contains(up.UserId));

            var usersToRemoveList = usersToRemove.ToList();

            if (!usersToRemoveList.Any())
                return Result<List<PartyMemberResponseDto>>.Failure("Users to remove could not be found");

            await _unitOfWork.PartyMember.RemoveUsersAsync(usersToRemoveList);

            var memberDtos = _mapper.Map<List<PartyMemberResponseDto>>(usersToRemoveList);
            return Result<List<PartyMemberResponseDto>>.Success(memberDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get party members");
            return Result<List<PartyMemberResponseDto>>.Failure("An error occurred while fetching party members.");
        }
    }

    public async Task<Result> UpdatePartyMemberRoles(string userId, int partyId, List<UpdateRoleDto> roleChanges)
    {
        try
        {
            var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId);
            if (party == null)
                return Result.Failure("Party not found or you do not have permission to update roles.");

            // Update roles
            var partyMembers = await _unitOfWork.PartyMember.GetAllAsync(pm => pm.PartyId == partyId);
            foreach (var roleChange in roleChanges)
            {
                var member = partyMembers.FirstOrDefault(pm => pm.UserId == roleChange.UserId);
                if (member == null)
                    continue;

                member.Role = roleChange.NewRole;
                if (roleChange.NewRole == UserRole.Leader)
                {
                    // Transfer leadership
                    party.CreatedById = roleChange.UserId;
                }
            }

            await _unitOfWork.PartyMember.UpdateUsersAsync(partyMembers);
            await _unitOfWork.Party.UpdateAsync(party);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update party member roles.");
            return Result.Failure("An error occurred while updating member roles.");
        }
    }


    public async Task<Result> LeaveParty(string userId, int partyId)
    {
        try
        {
            var foundPartyMember =
                await _unitOfWork.PartyMember.GetAsync(pm => pm.PartyId == partyId && pm.UserId == userId);
            if (foundPartyMember == null)
                return Result.Failure("You are not a member of this party.");

            var questAssignments =
                await _unitOfWork.QuestAssignment.GetAllAsync(qa => qa.UserId == userId && qa.Quest.PartyId == partyId);
            foreach (var assignment in questAssignments)
            {
                await _unitOfWork.QuestAssignment.RemoveAsync(assignment);
            }

            // Remove party member
            await _unitOfWork.PartyMember.RemoveAsync(foundPartyMember);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to leave the party.");
            return Result.Failure("An error occurred while leaving the party.");
        }
    }


    public async Task<Result> InvitePartyMember(string userId, int partyId, string inviteeEmail)
    {
        try
        {
            var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId);

            if (party == null)
                return Result.Failure("Party not found or you do not have permission to invite members.");

            // Generate token (for simplicity, we'll just use a GUID as a token)
            var token = Guid.NewGuid().ToString();
            var tokenExpiration = DateTime.UtcNow.AddMinutes(15); // Token expires after 15 minutes

            // Create an invite record in the database (store token and expiration date)
            var invite = new PartyMemberInvite
            {
                PartyId = partyId,
                InviterUserId = userId,
                InviteeEmail = inviteeEmail,
                Token = token,
                ExpirationDate = tokenExpiration
            };

            await _unitOfWork.PartyMemberInvite.CreateAsync(invite);

            // Send email with the invite link (simulating an email send here)
            var inviteLink = $"https://yourapp.com/party/invite/{token}";
            await _emailService.SendInviteEmailAsync(inviteeEmail, inviteLink);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invite party member.");
            return Result.Failure("An error occurred while sending the invite.");
        }
    }

    public async Task<Result> AcceptInvite(string token)
    {
        try
        {
            // Fetch the invite by token
            var invite = await _unitOfWork.PartyMemberInvite.GetAsync(i => i.Token == token);

            if (invite == null || invite.ExpirationDate < DateTime.UtcNow)
                return Result.Failure("Invalid or expired invite token.");

            // Check if user already a member of the party
            var isMember = await _unitOfWork.PartyMember.ExistsAsync(pm =>
                pm.PartyId == invite.PartyId && pm.UserId == invite.InviteeUserId);
            if (isMember)
                return Result.Failure("User is already a member of the party.");

            // Add the invitee as a party member
            var partyMember = new PartyMember
            {
                PartyId = invite.PartyId,
                UserId = invite.InviteeUserId,
                Role = UserRole.Member,
                JoinedAt = DateTime.UtcNow
            };
            await _unitOfWork.PartyMember.CreateAsync(partyMember);

            // Remove the invite to prevent re-use
            await _unitOfWork.PartyMemberInvite.RemoveAsync(invite);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to accept invite.");
            return Result.Failure("An error occurred while accepting the invite.");
        }
    }
}