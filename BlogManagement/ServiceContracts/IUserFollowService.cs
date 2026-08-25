using BlogManagement.DTO.Common;
using BlogManagement.DTO.UserFollow;

namespace BlogManagement.ServiceContracts;

public interface IUserFollowService
{
    Task<ApiResponse<object>> FollowUserAsync(CreateUserFollowRequestDTO requestDTO, string userEmail, CancellationToken ct);
    Task<ApiResponse<object>> UnfollowUserAsync(Guid authorId, string userEmail, CancellationToken ct);
}
