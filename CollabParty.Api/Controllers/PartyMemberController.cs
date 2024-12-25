using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/party-members")]
[ApiController]
[Authorize]
public class PartyMemberController : ControllerBase
{
    private readonly IPartyMemberService _partyMemberService;

    public PartyMemberController(IPartyMemberService partyMemberService)
    {
        _partyMemberService = partyMemberService;
    }

    [HttpGet("{partyId:int}")]
    public async Task<ActionResult<ApiResponse>> GetPartyMembers(int partyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _partyMemberService.GetPartyMembers(userId, partyId);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Data));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }

    [HttpPost("{partyId}/change-leader")]
    public async Task<ActionResult<ApiResponse>> ChangePartyLeader(
        int partyId,
        [FromBody] ChangePartyLeaderRequestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse.Error(
                "authorization",
                "Unauthorized access.",
                HttpStatusCode.Unauthorized));
        }

        var result = await _partyMemberService.ChangePartyLeader(partyId, dto);

        if (result.IsSuccess)
        {
            return Ok(ApiResponse.Success(result.Message));
        }

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }


    [HttpDelete("{partyId:int}/remove-members")]
    public async Task<ActionResult<ApiResponse>> RemovePartyMembers(int partyId, [FromBody] RemoverUserFromPartyDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _partyMemberService.RemovePartyMembers(userId, partyId, dto);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Data));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }

    [HttpPut("{partyId:int}/update-roles")]
    public async Task<ActionResult<ApiResponse>> UpdatePartyMembersRole(int partyId,
        [FromBody] List<UpdatePartyMemberRoleRequestDto> dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _partyMemberService.UpdatePartyMemberRoles(userId, partyId, dto);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Message));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }

    [HttpDelete("{partyId:int}/leave")]
    public async Task<ActionResult<ApiResponse>> LeaveParty(int partyId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _partyMemberService.LeaveParty(userId, partyId);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success());

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }
}