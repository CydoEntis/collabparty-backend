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
    public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterCredentialsDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Register(dto);

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

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginCredentialsDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(dto);
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

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse>> Logout([FromBody] TokenDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.Logout(dto);
            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse>> RefreshTokens([FromBody] TokenDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RefreshTokens(dto);
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

    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
                    HttpStatusCode.Unauthorized));

            var result = await _authService.ChangePasswordAsync(userId, dto);

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

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.SendForgotPasswordEmail(dto);

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
    public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(dto);

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