using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("/api/parties")]
[ApiController]
[Authorize]
public class QuestController : ControllerBase
{
    private readonly IQuestService _questService;
    private readonly IQuestCommentService _questCommentService;

    public QuestController(IQuestService questService, IQuestCommentService questCommentService)
    {
        _questService = questService;
        _questCommentService = questCommentService;
    }

    [HttpPost("quests")]
    public async Task<ActionResult<ApiResponse>> CreateQuest([FromBody] CreateQuestRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.CreateQuest(userId, dto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Message));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("internal", ex.InnerException?.Message ?? ex.Message,
                HttpStatusCode.InternalServerError);
        }
    }

    // Updated route to match /api/parties/{partyId}/quests
    [HttpGet("{partyId:int}/quests")]
    public async Task<ActionResult<ApiResponse>> GetAllQuests(int partyId, [FromQuery] QueryParamsDto query)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.GetQuests(userId, partyId, query); // Corrected here

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

    [HttpGet("quests/{questId:int}")]
    public async Task<ActionResult<ApiResponse>> GetAllQuests(int questId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.GetQuest(questId); // Corrected here

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

    [HttpPut("quests/{questId:int}/complete")]
    public async Task<ActionResult<ApiResponse>> CompleteQuest(int questId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.CompleteQuest(userId, questId);

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

    [HttpPut("quests/{questId:int}")]
    public async Task<ActionResult<ApiResponse>> UpdateQuest(int questId, [FromBody] UpdateQuestRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.UpdateQuest(userId, questId, dto);

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

    [HttpDelete("quests/{questId:int}")]
    public async Task<ActionResult<ApiResponse>> DeleteQuest(int questId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questService.DeleteQuest(userId, questId);

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

    [HttpPost("quests/{questId:int}/comments")]
    public async Task<ActionResult<ApiResponse>> AddComment(int questId, [FromBody] AddCommentRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            dto.QuestId = questId;
            dto.UserId = userId;

            var result = await _questCommentService.AddComment(dto);

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

    // Edit an existing comment
    [HttpPut("quests/{questId:int}/comments/{commentId:int}")]
    public async Task<ActionResult<ApiResponse>> EditComment(int questId, int commentId,
        [FromBody] EditCommentRequestDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            dto.Id = commentId;
            dto.UserId = userId;

            var result = await _questCommentService.EditComment(dto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success("Comment updated successfully."));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("internal", ex.InnerException?.Message ?? ex.Message,
                HttpStatusCode.InternalServerError);
        }
    }

    // Delete a comment
    [HttpDelete("quests/{questId:int}/comments/{commentId:int}")]
    public async Task<ActionResult<ApiResponse>> DeleteComment(int questId, int commentId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _questCommentService.DeleteComment(commentId, userId);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success("Comment deleted successfully."));

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