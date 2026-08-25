using BlogManagement.DTO.Comment;
using FluentValidation;

namespace BlogManagement.Validators.Comment;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequestDTO>
{
    public UpdateCommentRequestValidator()
    {
        RuleFor(x => x.CommentText)
            .NotNull().WithMessage("Comment is required.")
            .NotEmpty().WithMessage("Comment is required.")
            .Length(5, 1000).WithMessage("Comment must be between 5 and 1000 characters.");
    }
}
