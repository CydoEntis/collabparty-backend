using System.Net;
using CollabParty.Application.Common.Dtos;
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
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ApiResponse.Error("An unexpected error occurred.",
                    new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } }));
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
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ApiResponse.Error("An unexpected error occurred.",
                    new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } }));
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
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ApiResponse.Error("An unexpected error occurred.",
                    new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } }));
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
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ApiResponse.Error("An unexpected error occurred.",
                    new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } }));
        }
    }
}