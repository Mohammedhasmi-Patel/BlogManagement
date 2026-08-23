using System.Security.Claims;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/bookmarks")]
[Authorize]
public class BookmarkController : BaseController
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarkController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Guid BlogId)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("Invalid tpken.");
        var response = await _bookmarkService.CreateAsync(BlogId, userEmail);
        return Ok(response);
    }
}
