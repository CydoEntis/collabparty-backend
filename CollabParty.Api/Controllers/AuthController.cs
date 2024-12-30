using System.Security.Claims;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Errors;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Validators.Auth;
using CollabParty.Application.Common.Validators.Helpers;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ValidationHelper _validationHelper;

    public AuthController(IAuthService authService, ValidationHelper validationHelper)
    {
        _authService = authService;
        _validationHelper = validationHelper;
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto requestDto)
    {
        var validator = new RegisterCredentialsRequestDtoValidator();
        var results = validator.Validate(requestDto);

        if (!results.IsValid)
            return _validationHelper.HandleValidation(results.Errors);

        var result = await _authService.Register(requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result.Data));
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto requestDto)
    {
        var validator = new LoginCredentialsRequestDtoValidator();
        var results = validator.Validate(requestDto);

        if (!results.IsValid)
            return _validationHelper.HandleValidation(results.Errors);

        var result = await _authService.Login(requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result.Data));
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var result = await _authService.Logout();
        return Ok(ApiResponse<object>.SuccessResponse("Registration successful"));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens()
    {
        var result = await _authService.RefreshTokens();
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto requestDto)
    {
        var result = await _authService.SendForgotPasswordEmail(requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto requestDto)
    {
        var result = await _authService.ResetPasswordAsync(requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto requestDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User does not have permission");


        var result = await _authService.ChangePasswordAsync(userId, requestDto);
        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}