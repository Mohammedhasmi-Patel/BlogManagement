namespace BlogManagement.Models
{
    public class Bookmark
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public Guid BlogId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public AppUser User { get; set; } = null!;

        public Blog Blog { get; set; } = null!;

    }
}
