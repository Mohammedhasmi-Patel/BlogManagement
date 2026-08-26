using BlogManagement.DTO.Common;
using BlogManagement.DTO.Dropdown;
using BlogManagement.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Controllers;

[Route("api/authors/dropdown")]
public class DropDownController(IDropDownService dropDownService) : BaseController
{
    private readonly IDropDownService _dropDownService = dropDownService;

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginationResult<AuthorDropdownResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] GetAuthorDropdownRequestDTO requestDTO, CancellationToken ct)
    {
        var response = await _dropDownService.GetAuthorAsync(requestDTO, ct);
        return StatusCode(response.StatusCode, response);
    }
}
