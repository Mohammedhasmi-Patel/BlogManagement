namespace BlogManagement.DTO.Comment;

public class CreateCommentRequestDTO
{
    public Guid BlogId { get; set; }
    public string CommentText { get; set; } = string.Empty;
}
