using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/user")]
[ApiController]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserAvatarService _userAvatarService;
    private readonly IAvatarService _avatarService;

    public UserController(IUserService userService, IUserAvatarService userAvatarService, IAvatarService avatarService)
    {
        _userService = userService;
        _userAvatarService = userAvatarService;
        _avatarService = avatarService;
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateUserDetails([FromBody] UpdateUserRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _userService.UpdateUserDetails(userId, dto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Data));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpPut("avatars")]
    public async Task<ActionResult<ApiResponse>> UpdateUserAvatar([FromBody] UpdateUserAvatarRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _userService.UpdateUserAvatar(userId, dto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Data));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpGet("avatars/unlocked")]
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
    
    [HttpGet("avatars")]
    public async Task<ActionResult<ApiResponse>> GetAllAvatars()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                HttpStatusCode.Unauthorized));

        var result = await _userAvatarService.GetAllAvatars(userId);

        if (result.IsSuccess)
            return Ok(ApiResponse.Success(result.Data));

        var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
        return BadRequest(ApiResponse.ValidationError(formattedErrors));
    }

    [HttpGet("avatars/locked")]
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
    
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _userService.ChangePasswordAsync(userId, requestDto);

            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Message));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }
}