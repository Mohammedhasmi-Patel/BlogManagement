using BlogManagement.DTO.Comment;
using FluentValidation;

namespace BlogManagement.Validators.Comment;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequestDTO>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(x => x.BlogId).NotEmpty().WithMessage("Blog is required.");
        RuleFor(x => x.CommentText)
            .NotNull().WithMessage("Comment is required.")
            .Length(5, 1000).WithMessage("Comment must be between 5 and 1000 characters.");
    }
}
