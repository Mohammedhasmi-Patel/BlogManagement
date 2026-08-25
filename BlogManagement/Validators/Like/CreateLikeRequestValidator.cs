using BlogManagement.DTO.Like;
using FluentValidation;

namespace BlogManagement.Validators.Like;

public class CreateLikeRequestValidator : AbstractValidator<CreateLikeRequestDTO>
{
    public CreateLikeRequestValidator()
    {
        RuleFor(x => x.BlogId)
            .NotEmpty()
            .WithMessage("BlogId is required.");
    }
}
