using System.Security.Claims;
using BlogManagement.DTO.Bookmark;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/bookmarks")]
[Authorize]
public class BookmarkController(IBookmarkService bookmarkService) : BaseController
{
    private readonly IBookmarkService _bookmarkService = bookmarkService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookmarkRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("Invalid token.");
        var response = await _bookmarkService.CreateAsync(requestDTO.BlogId, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetBookmarkRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("Invalid token.");
        var response = await _bookmarkService.GetAllAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{blogId}")]
    public async Task<IActionResult> Delete([FromRoute] Guid blogId, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("Invalid token.");
        var response = await _bookmarkService.RemoveAsync(blogId, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}
