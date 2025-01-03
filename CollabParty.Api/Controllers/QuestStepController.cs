using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Dtos.QuestSteps;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("/api/steps")]
[ApiController]
[Authorize]
public class QuestStepController : ControllerBase
{
    private readonly IQuestStepService _questStepService;

    public QuestStepController(IQuestStepService questStepService)
    {
        _questStepService = questStepService;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateStepStatus([FromBody] QuestStepStatusDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questStepService.UpdateStepStatus(dto);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}