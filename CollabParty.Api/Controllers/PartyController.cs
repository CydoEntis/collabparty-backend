// using System.Net;
// using System.Security.Claims;
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
//     public PartyController(IPartyService partyService, IUserPartyService userPartyService)
//     {
//         _partyService = partyService;
//         
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
//     public async Task<ActionResult<ApiResponse>> GetParties()
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//     
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//     
//         var result = await _userPartyService.GetAllPartiesForUser(userId);
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
//     [HttpGet("{partyId:int}/members")]
//     public async Task<ActionResult<ApiResponse>> GetPartyMembers(int partyId)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//     
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//     
//         var result = await _userPartyService.GetPartyMembers(userId, partyId);
//     
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//     
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//     
//     [HttpDelete("{partyId:int}/members")]
//     public async Task<ActionResult<ApiResponse>> RemovePartyMembers(int partyId, [FromBody] RemoverUserFromPartyDto dto)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//     
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//     
//         var result = await _userPartyService.RemovePartyMembers(userId, partyId, dto);
//     
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//     
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
//     
//     [HttpPut("{partyId:int}/members/change-role")]
//     public async Task<ActionResult<ApiResponse>> UpdatePartyMembersRole(int partyId,
//         [FromBody] UpdatePartyMembersRoleDto dto)
//     {
//         var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//     
//         if (string.IsNullOrEmpty(userId))
//             return Unauthorized(ApiResponse.Error("authorization", "Unauthorized access.",
//                 HttpStatusCode.Unauthorized));
//     
//         var result = await _userPartyService.UpdatePartyMemberRoles(userId, partyId, dto);
//     
//         if (result.IsSuccess)
//             return Ok(ApiResponse.Success(result.Data));
//     
//         var formattedErrors = ValidationHelpers.FormatValidationErrors(result.Errors);
//         return BadRequest(ApiResponse.ValidationError(formattedErrors));
//     }
// }