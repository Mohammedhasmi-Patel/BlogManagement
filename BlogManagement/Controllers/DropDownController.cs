using System.Security.Claims;
using BlogManagement.DTO.Common;
using BlogManagement.DTO.Dropdown;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/dropdown")]
public class DropDownController(IDropDownService dropDownService) : BaseController
{
    private readonly IDropDownService _dropDownService = dropDownService;

    [HttpGet("author")]
    [ProducesResponseType(typeof(ApiResponse<PaginationResult<AuthorDropdownResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] GetAuthorDropdownRequestDTO requestDTO, CancellationToken ct)
    {
        var response = await _dropDownService.GetAuthorAsync(requestDTO, ct);
        return StatusCode(response.StatusCode, response);
    }

    [HttpGet("category")]
    [ProducesResponseType(typeof(ApiResponse<PaginationResult<CategoryDropDownResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategoryAsync([FromQuery] GetCategoryDropDownRequestDTO requestDTO, CancellationToken ct)
    {
        string userEmail = User.FindFirstValue(ClaimTypes.Email) ?? null;
                          
        var response = await _dropDownService.GetCategoryAsync(requestDTO,userEmail, ct);
        return StatusCode(response.StatusCode, response);
    }
}
