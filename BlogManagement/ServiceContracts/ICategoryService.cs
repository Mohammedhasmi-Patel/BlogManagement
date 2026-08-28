using BlogManagement.DTO.Category;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface ICategoryService
{
    Task<ApiResponse<CategoryResponseDTO>> CreateAsync(CreateCategoryRequestDTO requestDTO, string userEmail, CancellationToken ct);
    Task<ApiResponse<CategoryResponseDTO>> UpdateAsync(UpdateCategoryRequestDTO requestDTO, string userEmail, CancellationToken ct);

    Task<ApiResponse<CategoryResponseDTO>> GetByIdAsync(Guid id,string userEmail, CancellationToken ct);

    Task<ApiResponse<PaginationResult<CategoryResponseDTO>>> GetAllAsync(GetCategoriesRequestDTO requestDTO, string userEmail, CancellationToken ct);
}
