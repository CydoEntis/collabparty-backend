using AutoMapper;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Interfaces;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using CollabParty.Domain.Enums;
using CollabParty.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;

namespace CollabParty.Application.Services.Implementations;

public class PartyMemberService : IPartyMemberService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PartyMember> _logger;
    private readonly IMapper _mapper;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;

    public PartyMemberService(IUnitOfWork unitOfWork, ILogger<PartyMember> logger, IMapper mapper,
        IEmailService emailService, IEmailTemplateService emailTemplateService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
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
                Role = UserRole.Member,
                JoinedAt = DateTime.Now,
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
                Role = UserRole.Leader,
                JoinedAt = DateTime.Now,
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


    public async Task<InvitePartyMemberResponseDto> InvitePartyMember(string userId, int partyId, string inviteeEmail)
    {
        var party = await _unitOfWork.Party.GetAsync(p => p.Id == partyId && p.CreatedById == userId);

        if (EntityUtility.EntityIsNull(party))
            throw new NotFoundException("Party not found.");


        var token = Guid.NewGuid().ToString();
        var tokenExpiration = DateTime.UtcNow.AddMinutes(15);

        var invite = new PartyInvite
        {
            PartyId = partyId,
            InviterUserId = userId,
            InviteeEmail = inviteeEmail,
            Token = token,
            ExpirationDate = tokenExpiration
        };

        await _unitOfWork.PartyInvite.CreateAsync(invite);

        var inviteLink = $"https://yourapp.com/party/invite/{token}";

        var placeholders = new Dictionary<string, string>
        {
            { "Recipient's Email", inviteeEmail },
            { "Invite Link", inviteLink }
        };

        // TODO: Create accept invite email template.
        var emailBody = _emailTemplateService.GetEmailTemplate("AcceptInviteTemplate", placeholders);

        await _emailService.SendEmailAsync("Party Invite", inviteeEmail, inviteLink, emailBody);

        return new InvitePartyMemberResponseDto() { Message = "Invite to party has been sent." };
    }

    public async Task<AcceptInviteResponseDto> AcceptInvite(string token)
    {
        var invite = await _unitOfWork.PartyInvite.GetAsync(i => i.Token == token);

        if (EntityUtility.EntityIsNull(invite) || invite.ExpirationDate < DateTime.UtcNow)
            throw new InvalidTokenException("Invalid token.");

        var isMember = await _unitOfWork.PartyMember.ExistsAsync(pm =>
            pm.PartyId == invite.PartyId && pm.UserId == invite.InviteeUserId);
        if (isMember)
            throw new ConflictException("This party member is already accepted.");

        var partyMember = new PartyMember
        {
            PartyId = invite.PartyId,
            UserId = invite.InviteeUserId,
            Role = UserRole.Member,
            JoinedAt = DateTime.UtcNow
        };
        await _unitOfWork.PartyMember.CreateAsync(partyMember);

        await _unitOfWork.PartyInvite.RemoveAsync(invite);

        return new AcceptInviteResponseDto() { Message = "Party invite accepted.", PartyId = invite.PartyId };
    }
}