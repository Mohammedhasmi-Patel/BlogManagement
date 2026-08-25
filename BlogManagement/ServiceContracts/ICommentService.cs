using BlogManagement.DTO.Comment;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface ICommentService
{
    public Task<ApiResponse<object>> CreateCommentAsync(CreateCommentRequestDTO requestDTO, string userEmail, CancellationToken ct = default);
    public Task<ApiResponse<object>> UpdateCommentAsync(Guid commentId, UpdateCommentRequestDTO requestDTO, string userEmail, CancellationToken ct = default);
    public Task<ApiResponse<object>> DeleteCommentAsync(Guid commentId, string userEmail, CancellationToken ct = default);
}
