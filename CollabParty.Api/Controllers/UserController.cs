// using System.Net;
// using System.Security.Claims;
// using CollabParty.Application.Common.Dtos.Auth;
// using CollabParty.Application.Common.Dtos.Avatar;
// using CollabParty.Application.Common.Dtos.User;
// using CollabParty.Application.Common.Models;
// using CollabParty.Application.Common.Utility;
// using CollabParty.Application.Services.Interfaces;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace CollabParty.Api.Controllers;
//
// [Route("api/user")]
// [ApiController]
// [Authorize]
// public class UserController : ControllerBase
// {
//     private readonly IUserService _userService;
//
//     public UserController(IUserService userService)
//     {
//         _userService = userService;
//     }
//
//     [HttpPut]
//     public async Task<ActionResult<ApiResponse>> UpdateUserDetails([FromBody] UpdateUserRequestDto dto)
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
//             var result = await _userService.UpdateUserDetails(userId, dto);
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
//     public async Task<ActionResult<ApiResponse>> GetUserDetails()
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
//             var result = await _userService.GetUserDetails(userId);
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
// }