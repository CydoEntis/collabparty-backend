using System.Net;
using System.Security.Claims;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Dtos.Auth;
using CollabParty.Application.Common.Models;
using CollabParty.Application.Common.Utility;
using CollabParty.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterRequestDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
    
            var result = await _authService.Register(dto);
    
            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Message));
    
            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(requestDto);
            if (result.IsSuccess)
                return Ok(ApiResponse.Success(result.Message));

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        try
        {
            var result = await _authService.Logout();

            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success("Logged out successfully."));
            }

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("internal", ex.InnerException?.Message ?? ex.Message,
                HttpStatusCode.InternalServerError);
        }
    }


    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse>> RefreshTokens()
    {
        try
        {
            var result = await _authService.RefreshTokens();

            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success("Tokens refreshed successfully."));
            }

            var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
            return BadRequest(ApiResponse.ValidationError(formattedErrors));
        }
        catch (Exception ex)
        {
            return ApiResponse.Error("internal", ex.InnerException?.Message ?? ex.Message,
                HttpStatusCode.InternalServerError);
        }
    }


    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SendForgotPasswordEmail(requestDto);

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

    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(requestDto);

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

            var result = await _authService.ChangePasswordAsync(userId, requestDto);

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