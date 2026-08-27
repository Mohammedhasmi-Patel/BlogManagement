using BlogManagement.DTO.Comment;
using BlogManagement.DTO.Common;

namespace BlogManagement.ServiceContracts;

public interface ICommentService
{
    public Task<ApiResponse<CommentResponseDTO>> CreateCommentAsync(CreateCommentRequestDTO requestDTO, string userEmail, CancellationToken ct = default);
    public Task<ApiResponse<CommentResponseDTO>> UpdateCommentAsync(Guid commentId, UpdateCommentRequestDTO requestDTO, string userEmail, CancellationToken ct = default);
    public Task<ApiResponse<object>> DeleteCommentAsync(Guid commentId, string userEmail, CancellationToken ct = default);
}
