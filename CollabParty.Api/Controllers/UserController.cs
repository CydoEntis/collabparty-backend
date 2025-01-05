using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Dtos.Avatar;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Common.Validators.Helpers;
using CollabParty.Application.Common.Validators.User;
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
    private readonly ValidationHelper _validationHelper;

    public UserController(IUserService userService, ValidationHelper validationHelper)
    {
        _userService = userService;
        _validationHelper = validationHelper;
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserDetails([FromBody] UpdateUserRequestDto requestDto)
    {
        var validator = new UpdateUserRequestDtoValidator();
        var results = validator.Validate(requestDto);

        if (!results.IsValid)
            return _validationHelper.HandleValidation(results.Errors);

        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _userService.UpdateUserDetails(userId, requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet]
    public async Task<ActionResult> GetUserDetails()
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _userService.GetUserDetails(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpGet("stats")]
    public async Task<ActionResult> GetUserStats()
    {
        var (isValid, userId) = ClaimsHelper.TryGetUserId(User);
        if (!isValid)
            return Unauthorized("You do not have permission to access this resource.");

        var result = await _userService.GetUserStats(userId);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}