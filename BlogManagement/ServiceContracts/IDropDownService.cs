using BlogManagement.DTO.Common;
using BlogManagement.DTO.Dropdown;

namespace BlogManagement.ServiceContracts;

public interface IDropDownService
{
    Task<ApiResponse<PaginationResult<AuthorDropdownResponseDTO>>> GetAuthorAsync(GetAuthorDropdownRequestDTO? requestDTO = null, CancellationToken ct = default);
}
