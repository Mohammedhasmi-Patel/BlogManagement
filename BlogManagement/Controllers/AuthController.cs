using BlogManagement.DTO.Auth;
using BlogManagement.DTO.Common;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/auth")]
public class AuthController(IAuthService authService) : BaseController
{
    private readonly IAuthService _authService = authService;

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<RegisterUserResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUserAsync([FromForm] RegisterUserRequestDTO request, CancellationToken ct)
    {
        var response = await _authService.RegisterUserAsync(request, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("login")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginUserAsync([FromBody] LoginRequestDTO request, CancellationToken ct)
    {
        var response = await _authService.LoginUserAsync(request, ct);
        return StatusCode(StatusCodes.Status200OK, response);
    }
}
