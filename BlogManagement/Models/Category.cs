namespace BlogManagement.Models
{
    public class Category : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public Guid CreatedBy { get; set; }

    }
}
