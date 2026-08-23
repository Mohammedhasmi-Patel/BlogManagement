using System.Security.Claims;
using BlogManagement.DTO.Blog;
using BlogManagement.DTO.Common;
using BlogManagement.Enum;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/blogs")]
[Authorize(Roles = nameof(UserRoleEnum.Author))]
public class BlogController(IBlogService blogService) : BaseController
{
    private readonly IBlogService _blogService = blogService;

    [HttpPost("upload-image")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<UploadBlogImageResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UploadImageAsync([FromForm] IFormFile file, CancellationToken ct)
    {
        var response = await _blogService.UploadContentImageAsync(file, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<BlogResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateBlogRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email)
            ?? throw new UnauthorizedException("Unauthorized user.");

        var response = await _blogService.CreateAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}

