namespace BlogManagement.Models
{
    public class Like
    {
        public Guid Id { get; set; }

        public Guid BlogId { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Blog Blog { get; set; } = null!;

        public AppUser User { get; set; } = null!;

    }
}
