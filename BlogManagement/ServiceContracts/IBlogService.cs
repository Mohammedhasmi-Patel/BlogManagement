using BlogManagement.DTO.Blog;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface IBlogService
{
    Task<ApiResponse<UploadBlogImageResponseDTO>> UploadContentImageAsync(IFormFile? file, CancellationToken ct = default);
    Task<ApiResponse<BlogResponseDTO>> CreateAsync(CreateBlogRequestDTO requestDTO, string authorEmail, CancellationToken ct = default);
    Task<ApiResponse<PaginationResult<BlogResponseDTO>>> GetAllAsync(GetBlogsRequestDTO requestDTO, CancellationToken ct = default);
    Task<ApiResponse<BlogResponseDTO>> GetBySlugAsync(string slug, CancellationToken ct = default);
}
