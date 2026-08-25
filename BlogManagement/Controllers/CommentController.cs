using System.Security.Claims;
using BlogManagement.DTO.Comment;
using BlogManagement.DTO.Common;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/comments")]
[Authorize]
public class CommentController(ICommentService commentService) : BaseController
{
    private readonly ICommentService _commentService = commentService;

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]

    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequestDTO requestDTO, CancellationToken ct)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized Access!");
        var response = await _commentService.CreateCommentAsync(requestDTO, userEmail, ct);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPut("{commentId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateComment([FromRoute] Guid commentId, [FromBody] UpdateCommentRequestDTO requestDTO, CancellationToken ct)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized Access!");
        var response = await _commentService.UpdateCommentAsync(commentId, requestDTO, userEmail, ct);
        return StatusCode(StatusCodes.Status200OK, response);
    }

    [HttpDelete("{commentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId, CancellationToken ct)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized Access!");
        var response = await _commentService.DeleteCommentAsync(commentId, userEmail, ct);
        return Ok(response);
    }

}
