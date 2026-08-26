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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<PaginationResult<BlogResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetBlogsRequestDTO requestDTO, CancellationToken ct)
    {
        var response = await _blogService.GetAllAsync(requestDTO, ct);
        return StatusCode(response.StatusCode, response);
    }
}
