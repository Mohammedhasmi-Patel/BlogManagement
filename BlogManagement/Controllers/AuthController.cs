using BlogManagement.DTO.Auth;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUserAsync([FromForm] RegisterUserRequestDTO request)
    {
        var response = await _authService.RegisterUserAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUserAsync( LoginRequestDTO request)
    {
        var response = await _authService.LoginUserAsync(request);
        return Ok(response);
    }
}
