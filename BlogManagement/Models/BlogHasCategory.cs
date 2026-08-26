namespace BlogManagement.Models;

public class BlogHasCategory
{
    public Guid BlogId { get; set; }
    public Guid CategoryId { get; set; }

    public Blog Blog { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
