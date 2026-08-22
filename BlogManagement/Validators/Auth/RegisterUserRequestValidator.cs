using BlogManagement.DTO.Auth;
using FluentValidation;

namespace BlogManagement.Validators.Auth;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequestDTO>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid user role.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(x => x.Password).WithMessage("Passwords do not match.");

        RuleFor(x => x.Avatar)
            .Must(file => file == null || new[] { ".jpg", ".jpeg", ".png" }
                .Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase)).WithMessage("Only .jpg, .jpeg, and .png files are allowed.");
        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));
    }
}
