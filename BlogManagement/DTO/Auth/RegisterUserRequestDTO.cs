namespace BlogManagement.DTO.Auth;

public class RegisterUserRequestDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Bio { get; set; }
}
