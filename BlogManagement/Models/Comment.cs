namespace BlogManagement.Models;

public class Comment : BaseEntity
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid BlogId { get; set; }
    public Guid UserId { get; set; }
    // Navigation properties
    public Blog Blog { get; set; } = null!;

    public AppUser User { get; set; } = null!;

}
