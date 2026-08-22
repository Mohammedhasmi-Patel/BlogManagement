using BlogManagement.DTO.Category;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface ICategoryService
{
    public Task<ApiResponse<CategoryResponseDTO>> CreateAsync(CreateCategoryRequestDTO requestDTO,string userEmail,CancellationToken ct);
    public Task<ApiResponse<PaginationResult<CategoryResponseDTO>>> GetAllAsync(GetCategoriesRequestDTO requestDTO,string userEmail,CancellationToken ct);
}
