using CollabParty.Application.Common.Dtos.Party;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/party")]
[ApiController]
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
        
        var result = await _partyService.
    }
}