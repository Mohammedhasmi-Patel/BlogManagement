using System.Security.Claims;
using BlogManagement.DTO.Blog;
using BlogManagement.DTO.Common;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/user/blogs")]
[Produces("application/json")]
public class UserBlogController(IBlogService blogService) : BaseController
{
    private readonly IBlogService _blogService = blogService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginationResult<BlogResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetBlogsRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var response = await _blogService.GetAllAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ApiResponse<BlogResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlugAsync([FromRoute] string slug, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var response = await _blogService.GetBySlugAsync(slug, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}
