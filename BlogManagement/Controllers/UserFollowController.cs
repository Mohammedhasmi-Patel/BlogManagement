using System.Security.Claims;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.UserFollow;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/follows")]
[Authorize]
[Produces("application/json")]
public class UserFollowController(IUserFollowService userFollowService) : BaseController
{
    private readonly IUserFollowService _userFollowService = userFollowService;

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<FollowAuthorResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> FollowUser([FromBody] CreateUserFollowRequestDTO requestDTO, CancellationToken ct)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized Access!");
        var response = await _userFollowService.FollowUserAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete("{authorId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FollowAuthorResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnfollowUser([FromRoute] Guid authorId, CancellationToken ct)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized Access!");
        var response = await _userFollowService.UnfollowUserAsync(authorId, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}

