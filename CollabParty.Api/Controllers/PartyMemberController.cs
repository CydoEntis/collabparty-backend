using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Common.Validators.Helpers;
using CollabParty.Application.Services.Interfaces;
using CollabParty.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/party-members")]
[ApiController]
[Authorize]
public class PartyMemberController : ControllerBase
{
    private readonly IPartyMemberService _partyMemberService;
    private readonly ValidationHelper _validationHelper;

    public PartyMemberController(IPartyMemberService partyMemberService, ValidationHelper validationHelper)
    {
        _partyMemberService = partyMemberService;
        _validationHelper = validationHelper;
    }

    [HttpGet("{partyId:int}")]
    public async Task<ActionResult> GetPartyMembers(int partyId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _partyMemberService.GetPartyMembers(userId, partyId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpPut("{partyId}/change-leader")]
    public async Task<ActionResult> ChangePartyLeader(
        int partyId,
        [FromBody] ChangePartyLeaderRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _partyMemberService.ChangePartyLeader(partyId, dto);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpPut("{partyId}/members")]
    public async Task<ActionResult> UpdatePartyMembers(
        int partyId,
        [FromBody] List<MemberUpdateDto> dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _partyMemberService.UpdatePartyMembers(partyId, dto);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpDelete("{partyId:int}/leave")]
    public async Task<ActionResult> LeaveParty(int partyId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _partyMemberService.LeaveParty(userId, partyId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("{partyId}/invite")]
    public async Task<ActionResult> InvitePartyMember(int partyId, [FromBody] PartyInviteRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _partyMemberService.InvitePartyMember(userId, partyId, dto.InviteeEmail);


        return Ok(ApiResponse<string>.SuccessResponse(result.Message));
    }

    [HttpPost("accept-invite")]
    public async Task<ActionResult> AcceptInvite([FromBody] AcceptInviteRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _partyMemberService.AcceptInvite(userId, dto.Token);


        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}