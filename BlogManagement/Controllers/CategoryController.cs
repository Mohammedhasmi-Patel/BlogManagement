using System.Security.Claims;
using BlogManagement.DTO.Category;
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
    public async Task<IActionResult> CreateAsync([FromForm] CreateCategoryRequestDTO requestDTO, CancellationToken ct)
    {
        var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? throw new BadRequestException("invalid token.");
        var response = await _categoryService.CreateAsync(requestDTO, userEmail, ct);
        return Ok(response);
    }
}
