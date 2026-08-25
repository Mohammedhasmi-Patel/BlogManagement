using BlogManagement.DTO.UserFollow;
using FluentValidation;

namespace BlogManagement.Validators.UserFollow;

public class CreateUserFollowRequestValidator : AbstractValidator<CreateUserFollowRequestDTO>
{
    public CreateUserFollowRequestValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotNull().WithMessage("Author ID is required.")
            .NotEmpty().WithMessage("Author ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("Author ID must be a valid GUID.");
    }
}
