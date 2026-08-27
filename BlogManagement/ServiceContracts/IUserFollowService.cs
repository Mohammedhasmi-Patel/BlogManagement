using BlogManagement.DTO.Common;
using BlogManagement.DTO.UserFollow;

namespace BlogManagement.ServiceContracts;

public interface IUserFollowService
{
    Task<ApiResponse<FollowAuthorResponseDTO>> FollowUserAsync(CreateUserFollowRequestDTO requestDTO, string userEmail, CancellationToken ct);
    Task<ApiResponse<FollowAuthorResponseDTO>> UnfollowUserAsync(Guid authorId, string userEmail, CancellationToken ct);
}
