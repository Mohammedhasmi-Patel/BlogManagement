using BlogManagement.DTO.Bookmark;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface IBookmarkService
{
    Task<ApiResponse<object>> CreateAsync(Guid BlogId, string email, CancellationToken ct = default);
    Task<ApiResponse<object>> RemoveAsync(Guid blogId, string email, CancellationToken ct = default);
    Task<ApiResponse<PaginationResult<BookmarkBlogListingresponseDTO>>> GetAllAsync(GetBookmarkRequestDTO requestDTO, string email, CancellationToken ct = default);
}
