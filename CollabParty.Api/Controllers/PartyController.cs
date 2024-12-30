// using System.Net;
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
// [Route("api/parties")]
// [ApiController]
// [Authorize]
// public class PartyController : ControllerBase
// {
//     private readonly IPartyService _partyService;
//
//     public PartyController(IPartyService partyService)
//     {
//         _partyService = partyService;
//     }
//
//     [HttpPost]
//     public async Task<ActionResult<ApiResponse>> CreateParty([FromBody] CreatePartyDto dto)
//     {
//         try
//         {
//             if (!ModelState.IsValid)
//                 return BadRequest(ModelState);
//
//             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//             if (string.IsNullOrEmpty(userId))
//                 return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                     HttpStatusCode.Unauthorized));
//
//             var result = await _partyService.CreateParty(userId, dto);
//
//             if (result.IsSuccess)
//                 return Ok(ApiResponse.Success(result.Data));
//
//             var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//             return BadRequest(ApiResponse.ValidationError(formattedErrors));
//         }
//         catch (Exception ex)
//         {
//             return
//                 ApiResponse.Error("internal", ex.InnerException.Message, HttpStatusCode.InternalServerError);
//         }
//     }
//
//     [HttpGet]
//     public async Task<ActionResult<ApiResponse>> GetParties([FromQuery] QueryParamsDto query)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _partyService.GetAllPartiesForUser(userId, query);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//
//     [HttpGet("recent")]
//     public async Task<ActionResult<ApiResponse>> GetRecentParties()
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _partyService.GetRecentParties(userId);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
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
//         var result = await _partyService.GetParty(userId, partyId);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//     [HttpPut("{partyId:int}")]
//     public async Task<ActionResult<ApiResponse>> UpdateParty(int partyId, [FromBody] UpdatePartyDto dto)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _partyService.UpdateParty(userId, partyId, dto);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//
//     [HttpDelete("{partyId:int}")]
//     public async Task<ActionResult<ApiResponse>> DeleteParty(int partyId)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//
//         var result = await _partyService.DeleteParty(userId, partyId);
//
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
// }