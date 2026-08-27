namespace BlogManagement.DTO.UserFollow;

public class FollowAuthorResponseDTO
{
    public string AuthorId { get; set; } = string.Empty;
    public bool IsFollowing { get; set; } = false;
    public int TotalFollowers { get; set; } = 0;
}
