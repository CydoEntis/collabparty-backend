using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Common.Validators.Helpers;
using CollabParty.Application.Common.Validators.Party;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;
using UnauthorizedException = CollabParty.Application.Common.Errors.UnauthorizedException;

namespace CollabParty.Api.Controllers;

[Route("api/parties")]
[ApiController]
[Authorize]
public class PartyController : ControllerBase
{
    private readonly IPartyService _partyService;
    private readonly ValidationHelper _validationHelper;

    public PartyController(IPartyService partyService, ValidationHelper validationHelper)
    {
        _partyService = partyService;
        _validationHelper = validationHelper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateParty([FromBody] CreatePartyRequestDto requestDto)
    {
        var validator = new CreatePartyRequestDtoValidator();
        var results = validator.Validate(requestDto);

        if (!results.IsValid)
            return _validationHelper.HandleValidation(results.Errors);

        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");


        var result = await _partyService.CreateParty(userId, requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet]
    public async Task<ActionResult> GetParties([FromQuery] QueryParamsDto query)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _partyService.GetAllPartiesForUser(userId, query);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpGet("recent")]
    public async Task<ActionResult> GetRecentParties()
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _partyService.GetRecentParties(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpGet("{partyId:int}")]
    public async Task<ActionResult> GetParty(int partyId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _partyService.GetParty(userId, partyId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("{partyId:int}")]
    public async Task<ActionResult> UpdateParty(int partyId, [FromBody] UpdatePartyRequestDto requestDto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _partyService.UpdateParty(userId, partyId, requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpDelete("{partyId:int}")]
    public async Task<ActionResult> DeleteParty(int partyId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _partyService.DeleteParty(userId, partyId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}