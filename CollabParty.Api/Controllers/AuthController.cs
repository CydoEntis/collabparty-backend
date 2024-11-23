using System.Net;
using CollabParty.Application.Common.Dtos;
using CollabParty.Application.Common.Models;
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
            var result = await _authService.Register(dto);
            if (result.IsSuccess)
            {
                return Ok(ApiResponse.Success(result.Data));
            }

            var errors = new Dictionary<string, List<string>>
            {
                { "email", new List<string> { result.ErrorMessage } }
            };

            return BadRequest(ApiResponse.ValidationError(errors));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ApiResponse.Error("An unexpected error occurred.",
                    new Dictionary<string, List<string>> { { "Exception", new List<string> { ex.Message } } }));
        }
    }
}