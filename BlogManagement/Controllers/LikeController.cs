using System.Security.Claims;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.Like;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/likes")]
[Authorize]
public class LikeController(ILikeService likeService) : BaseController
{
    private readonly ILikeService _likeService = likeService;

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateLike([FromBody] CreateLikeRequestDTO requestDTO)
    {
        string? userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("Invalid token");
        var response = await _likeService.CreateLikeAsync(requestDTO, userEmail);
        return StatusCode(StatusCodes.Status201Created, response);
    }


    [HttpDelete("{blogId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveLike([FromRoute] Guid blogId)
    {
        string? userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("Invalid token");
        var response = await _likeService.RemoveLikeAsync(blogId, userEmail);
        return StatusCode(StatusCodes.Status200OK, response);
    }
}
