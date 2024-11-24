using System.Net;
using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/party")]
[ApiController]
[Authorize]
public class PartyController : ControllerBase
{
    private readonly IPartyService _partyService;

    public PartyController(IPartyService partyService)
    {
        _partyService = partyService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateParty([FromBody] CreatePartyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst("sub")?.Value;
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.", HttpStatusCode.Unauthorized));

        var result = await _partyService.CreateParty(userId, dto);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Data));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }
}