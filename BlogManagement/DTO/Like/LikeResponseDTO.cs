namespace BlogManagement.DTO.Like;

public class LikeResponseDTO
{
    public Guid BlogId { get; set; }
    public bool IsLiked { get; set; }
    public int LikeCount { get; set; }
    public Guid? LikeId { get; set; }
}
