namespace BlogManagement.Models;

public class UserFollow
{
    public Guid FollowerId { get; set; }
    public Guid AuthorId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public AppUser Follower { get; set; } = null!;

    public AppUser Author { get; set; } = null!;

}
