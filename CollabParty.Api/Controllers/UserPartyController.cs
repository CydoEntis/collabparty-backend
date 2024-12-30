﻿// using System.Net;
// using System.Security.Claims;
// using CollabParty.Application.Common.Dtos;
// using CollabParty.Application.Common.Dtos.Party;
// using CollabParty.Application.Common.Dtos.User;
// using CollabParty.Application.Common.Models;
// using CollabParty.Application.Common.Utility;
// using CollabParty.Application.Services.Interfaces;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace CollabParty.Api.Controllers;
//
// [Route("api/user-parties")]
// [ApiController]
// [Authorize]
// public class UserPartyController : ControllerBase
// {
//     private readonly IPartyService _partyService;
//     private readonly IUserPartyService _userPartyService;
//
//     public UserPartyController(IPartyService partyService, IUserPartyService userPartyService)
//     {
//         _partyService = partyService;
//         _userPartyService = userPartyService;
//     }
//
//     [HttpPost]
//     public async Task<ActionResult<ApiResponse>> CreateParty([FromBody] CreatePartyDto dto)
//     {
//         if (!ModelState.IsValid)
//             return BadRequest(ModelState);
//
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _partyService.CreateParty(userId, dto);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//     [HttpGet]
//     public async Task<ActionResult<ApiResponse>> GetParties([FromQuery] QueryParamsDto dto)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _userPartyService.GetAllPartiesForUser(userId, dto);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//     [HttpGet("most-recent")]
//     public async Task<ActionResult<ApiResponse>> GetMostRecentParties()
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _userPartyService.GetRecentParties(userId);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//     [HttpGet("{partyId:int}")]
//     public async Task<ActionResult<ApiResponse>> GetParty(int partyId)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _userPartyService.GetParty(userId, partyId);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//
// }