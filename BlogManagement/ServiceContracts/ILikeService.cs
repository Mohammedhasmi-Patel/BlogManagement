using BlogManagement.DTO.Common;
using BlogManagement.DTO.Like;

namespace BlogManagement.ServiceContracts;

public interface ILikeService
{
    Task<ApiResponse<object>> CreateLikeAsync(CreateLikeRequestDTO requestDTO, string userEmail, CancellationToken ct = default);
    Task<ApiResponse<object>> RemoveLikeAsync(Guid blogId, string userEmail, CancellationToken ct = default);
}

