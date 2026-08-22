using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
}
