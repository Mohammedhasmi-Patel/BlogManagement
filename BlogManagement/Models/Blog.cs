namespace BlogManagement.Models;

public class Blog
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string Status { get; set; } = "draft";

    public string? SeoJson { get; set; }

    public int ViewCount { get; set; } = 0;

    public int ReadingTimeMinutes { get; set; } = 0;

    public Guid AuthorId { get; set; }

    public DateTime? PublishedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public AppUser Author { get; set; } = null!;

    public ICollection<BlogHasCategory> BlogCategories { get; set; } = new List<BlogHasCategory>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public ICollection<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();

    public ICollection<BlogHasMedia> Media { get; set; } = new List<BlogHasMedia>();
}
