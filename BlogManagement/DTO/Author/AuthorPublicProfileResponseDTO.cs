namespace BlogManagement.DTO.Author;

public class AuthorPublicProfileResponseDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public int TotalFollowers { get; set; }
    public bool IsFollowing { get; set; }
    public int TotalPosts { get; set; }
}
