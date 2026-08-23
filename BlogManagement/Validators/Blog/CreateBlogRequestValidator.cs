using BlogManagement.DTO.Blog;
using FluentValidation;

namespace BlogManagement.Validators.Blog;

public class CreateBlogRequestValidator : AbstractValidator<CreateBlogRequestDTO>
{
    public CreateBlogRequestValidator()
    {
        RuleFor(b => b.Title)
            .NotEmpty()
            .WithMessage("Blog title is required.")
            .MinimumLength(3)
            .WithMessage("Blog title must be at least 3 characters long.")
            .MaximumLength(255)
            .WithMessage("Blog title must not exceed 255 characters.");

        RuleFor(b => b.Content)
            .NotEmpty()
            .WithMessage("Blog content is required.")
            .MinimumLength(10)
            .WithMessage("Blog content must be at least 10 characters long.");

        RuleFor(b => b.Status)
            .Must(status => string.Equals(status, "draft", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(status, "published", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Blog status must be either 'draft' or 'published'.");

        When(b => !string.IsNullOrWhiteSpace(b.Summary), () =>
        {
            RuleFor(b => b.Summary)
                .MaximumLength(500)
                .WithMessage("Summary must not exceed 500 characters.");
        });

        When(b => b.CoverImage != null, () =>
        {
            RuleFor(b => b.CoverImage!)
                .Must(file => file.Length <= 5 * 1024 * 1024)
                .WithMessage("Cover image file size must not exceed 5MB.")
                .Must(file => new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" }
                    .Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage("Only .jpg, .jpeg, .png, .webp, and .svg files are allowed for cover image.");
        });

        RuleForEach(b => b.CategoryIds)
            .NotEmpty()
            .WithMessage("Invalid category ID specified.");
    }
}
