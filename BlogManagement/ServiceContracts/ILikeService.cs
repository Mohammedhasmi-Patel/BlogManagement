using BlogManagement.DTO.Common;
using BlogManagement.DTO.Like;

namespace BlogManagement.ServiceContracts;

public interface ILikeService
{
    Task<ApiResponse<LikeResponseDTO>> CreateLikeAsync(CreateLikeRequestDTO requestDTO, string userEmail, CancellationToken ct = default);
    Task<ApiResponse<LikeResponseDTO>> RemoveLikeAsync(Guid blogId, string userEmail, CancellationToken ct = default);
}

