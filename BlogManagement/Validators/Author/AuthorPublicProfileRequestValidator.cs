using BlogManagement.DTO.Author;
using FluentValidation;

namespace BlogManagement.Validators.Author;

public class AuthorPublicProfileRequestValidator : AbstractValidator<AuthorPublicProfileRequestDTO>
{
    public AuthorPublicProfileRequestValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("Author ID is required.")
            .NotEmpty().WithMessage("Author ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Author ID must be a valid GUID.");
    }
}
