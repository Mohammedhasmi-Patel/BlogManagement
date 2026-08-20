namespace BlogManagement.Models;

public class BlogHasMedia
{
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? MimeType { get; set; }

    public long? FileSize { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public bool IsPrimary { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Blog? Blog { get; set; }
}
