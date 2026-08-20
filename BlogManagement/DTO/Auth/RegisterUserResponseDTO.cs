namespace BlogManagement.DTO.Auth;

public class RegisterUserResponseDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; }
}
