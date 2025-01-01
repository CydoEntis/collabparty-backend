using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Common.Validators.Helpers;
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
    private readonly ValidationHelper _validationHelper;

    public QuestController(IQuestService questService, IQuestCommentService questCommentService)
    {
        _questService = questService;
        _questCommentService = questCommentService;
    }

    [HttpPost("quests")]
    public async Task<ActionResult> CreateQuest([FromBody] CreateQuestRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questService.CreateQuest(userId, dto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    // Updated route to match /api/parties/{partyId}/quests
    [HttpGet("{partyId:int}/quests")]
    public async Task<ActionResult> GetAllQuests(int partyId, [FromQuery] QueryParamsDto query)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questService.GetQuests(userId, partyId, query); // Corrected here

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("quests/{questId:int}")]
    public async Task<ActionResult> GetAllQuests(int questId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questService.GetQuest(questId); // Corrected here

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("quests/{questId:int}/complete")]
    public async Task<ActionResult> CompleteQuest(int questId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questService.CompleteQuest(userId, questId);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("quests/{questId:int}")]
    public async Task<ActionResult> UpdateQuest(int questId, [FromBody] UpdateQuestRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questService.UpdateQuest(userId, questId, dto);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpDelete("quests/{questId:int}")]
    public async Task<ActionResult> DeleteQuest(int questId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questService.DeleteQuest(userId, questId);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpGet("quests/{questId:int}/comments")]
    public async Task<ActionResult> GetPaginatedComments(int questId, [FromQuery] QueryParamsDto query)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questCommentService.GetPaginatedComments(questId, query);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("quests/{questId:int}/comments")]
    public async Task<ActionResult> AddComment(int questId, [FromBody] AddQuestCommentRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        dto.QuestId = questId;
        dto.UserId = userId;

        var result = await _questCommentService.AddComment(dto);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }


    [HttpDelete("quests/{questId:int}/comments/{commentId:int}")]
    public async Task<ActionResult> DeleteComment(int questId, int commentId)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have access to this resource.");

        var result = await _questCommentService.DeleteComment(commentId, userId);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}