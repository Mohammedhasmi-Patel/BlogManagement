using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface IBookmarkService
{
    Task<ApiResponse<object>> CreateAsync(Guid BlogId, string email);
}
