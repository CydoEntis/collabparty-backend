using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/avatars")]
[ApiController]
[Authorize]
public class AvatarController : ControllerBase
{
    private readonly IAvatarService _avatarService;
    private readonly IUnlockedAvatarService _unlockedAvatarService;

    public AvatarController(IAvatarService avatarService, IUnlockedAvatarService unlockedAvatarService)
    {
        _avatarService = avatarService;
        _unlockedAvatarService = unlockedAvatarService;
    }

    [HttpGet("locked")]
    public async Task<ActionResult> GetLockedAvatars()
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _avatarService.GetLockedAvatars(userId);

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("unlocked")]
    public async Task<ActionResult> GetUnlockedAvatars()
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _unlockedAvatarService.GetUnlockedAvatars(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("unlockable")]
    public async Task<ActionResult> GetUnlockableAvatars()
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _unlockedAvatarService.GetUnlockableAvatars(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPut("active")]
    public async Task<ActionResult> SetActiveAvatar([FromBody] SelectedAvatarRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);


        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _unlockedAvatarService.SetActiveAvatar(userId, dto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("unlock")]
    public async Task<ActionResult> UnlockAvatar([FromBody] SelectedAvatarRequestDto dto)
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _unlockedAvatarService.UnlockAvatar(userId, dto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}