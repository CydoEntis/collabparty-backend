using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/avatars")]
[ApiController]
[Authorize]
public class UserAvatarController : ControllerBase
{
    private readonly IUserAvatarService _userAvatarService;
    private readonly IAvatarService _avatarService;

    public UserAvatarController(IUserAvatarService userAvatarService, IAvatarService avatarService)
    {
        _userAvatarService = userAvatarService;
        _avatarService = avatarService;
    }

    [HttpGet("unlocked")]
    public async Task<ActionResult<ApiResponse>> GetAllUnlockedAvatars()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _userAvatarService.GetAllUnlockedAvatars(userId);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Data));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }

    [HttpGet("locked")]
    public async Task<ActionResult<ApiResponse>> GetAllLockedAvatars()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _avatarService.GetLockedAvatars(userId);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Data));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }
}