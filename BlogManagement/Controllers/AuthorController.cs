using System.Security.Claims;
using BlogManagement.DTO.Author;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/users")]
[Produces("application/json")]
public class AuthorController(IAuthorService authorService) : BaseController
{
    private readonly IAuthorService _authorService = authorService;

    [HttpGet("authors")]
    [ProducesResponseType(typeof(ApiResponse<AuthorPublicProfileResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuthorPublicProfileAsync([FromQuery] AuthorPublicProfileRequestDTO requestDTO, CancellationToken ct)
    {
        string? userEmail = User.FindFirstValue(ClaimTypes.Email);
        var response = await _authorService.GetAuthorPublicProfileAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}


