using System.Security.Claims;
using BlogManagement.DTO.Category;
using BlogManagement.DTO.Common;
using BlogManagement.Enum;
using BlogManagement.Exceptions;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/categories")]
[Authorize(Roles = nameof(UserRoleEnum.Author))]
public class CategoryController(ICategoryService categoryService) : BaseController
{
    private readonly ICategoryService _categoryService = categoryService;

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync([FromForm] CreateCategoryRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized user.");
        var response = await _categoryService.CreateAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginationResult<CategoryResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetCategoriesRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized user.");
        var response = await _categoryService.GetAllAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized user.");
        var response = await _categoryService.GetByIdAsync(id, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromForm] UpdateCategoryRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized user.");
        requestDTO.Id = id;
        var response = await _categoryService.UpdateAsync(requestDTO, userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}
