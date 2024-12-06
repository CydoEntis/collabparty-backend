﻿using System.Net;
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
    public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDto requestDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(requestDto);
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
    public async Task<ActionResult<ApiResponse>> Logout([FromBody] TokenResponseDto responseDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.Logout(responseDto);
            return Ok(ApiResponse.Success());
        }
        catch (Exception ex)
        {
            return
                ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse>> RefreshTokens([FromBody] TokenResponseDto responseDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RefreshTokens(responseDto);
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
}