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
    public async Task<ActionResult<ApiResponse>> UpdateStepStatus([FromBody] QuestStepStatusDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questStepService.UpdateStepStatus(dto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Data));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("internal", ex.InnerException?.Message ?? ex.Message,
                HttpStatusCode.InternalServerError);
        }
    }
}