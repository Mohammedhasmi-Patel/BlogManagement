namespace BlogManagement.DTO.Blog;

public class BlogMediaResponseDTO
{
    public Guid Id { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string? FileName { get; set; }

    public string? FileUrl { get; set; }

    public string? MimeType { get; set; }

    public long? FileSize { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }
}
