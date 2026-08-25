using BlogManagement.DTO.Auth;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService) : BaseController
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUserAsync([FromForm] RegisterUserRequestDTO request)
    {
        var response = await _authService.RegisterUserAsync(request);
        return Ok(response);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUserAsync([FromBody] LoginRequestDTO request)
    {
        var response = await _authService.LoginUserAsync(request);
        return Ok(response);
    }
}
