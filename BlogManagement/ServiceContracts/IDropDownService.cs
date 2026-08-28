using BlogManagement.DTO.Common;
using BlogManagement.DTO.Dropdown;

namespace BlogManagement.ServiceContracts;

public interface IDropDownService
{
    Task<ApiResponse<PaginationResult<AuthorDropdownResponseDTO>>> GetAuthorAsync(GetAuthorDropdownRequestDTO? requestDTO = null, CancellationToken ct = default);
    Task<ApiResponse<PaginationResult<CategoryDropDownResponseDTO>>> GetCategoryAsync(GetCategoryDropDownRequestDTO? requestDTO = null, string? userEmail = null, CancellationToken ct = default);
}
