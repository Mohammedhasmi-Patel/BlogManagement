using Microsoft.AspNetCore.Identity;

namespace BlogManagement.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
    }
}
