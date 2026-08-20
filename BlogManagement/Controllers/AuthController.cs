using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthControllerController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
