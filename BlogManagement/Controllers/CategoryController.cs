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
public class CategoryController : BaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

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
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ApiResponse<CategoryResponseDTO>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetAllAsync(GetCategoriesRequestDTO requestDTO,CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedException("Unauthorized user.");
        var response = await _categoryService.GetAllAsync(requestDTO,userEmail,ct);  
        return Ok(response);
    }
}
