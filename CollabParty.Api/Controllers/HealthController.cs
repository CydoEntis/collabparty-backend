using Microsoft.AspNetCore.Mvc;

namespace CollabParty.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    [Route("status")] // Endpoint: /api/health/status
    public IActionResult GetStatus()
    {
        return Ok(new { status = "Healthy", timestamp = DateTime.UtcNow });
    }
}