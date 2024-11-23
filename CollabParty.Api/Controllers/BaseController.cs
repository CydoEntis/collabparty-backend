using System.Net;
using CollabParty.Application.Common.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected ActionResult<ApiResponse> OkResponse(object result)
    {
        var response = new ApiResponse
        {
            StatusCode = HttpStatusCode.OK,
            IsSuccess = true,
            Result = result
        };
        return Ok(response);
    }
    
    protected ActionResult<ApiResponse> CreatedResponse(object result)
    {
        var response = new ApiResponse
        {
            StatusCode = HttpStatusCode.Created,
            IsSuccess = true,
            Result = result
        };
        return Ok(response);
    }
    
    protected ActionResult<ApiResponse> BadRequestResponse(Dictionary<string, List<string>> errors)
    {
        var response = new ApiResponse
        {
            StatusCode = HttpStatusCode.BadRequest,
            IsSuccess = false,
            Errors = errors
        };
        return BadRequest(response);
    }

    protected ActionResult<ApiResponse> InternalServerErrorResponse(Dictionary<string, List<string>> errors)
    {
        var response = new ApiResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccess = false,
            Errors = errors
        };
        return StatusCode((int)HttpStatusCode.InternalServerError, response);
    }
}