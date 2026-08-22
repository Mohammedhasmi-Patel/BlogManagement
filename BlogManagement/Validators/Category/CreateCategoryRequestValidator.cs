using BlogManagement.DTO.Category;
using FluentValidation;

namespace BlogManagement.Validators.Category;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDTO>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(ct => ct.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MinimumLength(3)
            .WithMessage("Category name must be at least 3 characters long.")
            .MaximumLength(55)
            .WithMessage("Category name must not exceed 55 characters.");

        When(ct => !string.IsNullOrWhiteSpace(ct.Description), () =>
        {
            RuleFor(ct => ct.Description)
                .MinimumLength(10)
                .WithMessage("Description must be at least 10 characters long.")
                .MaximumLength(255)
                .WithMessage("Description must not exceed 255 characters.");
        });

        When(x => x.Icon != null, () =>
        {
            RuleFor(x => x.Icon!)
                .Must(file => file.Length <= 5 * 1024 * 1024)
                .WithMessage("Icon file size must not exceed 5MB.")
                .Must(file => new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" }
                    .Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage("Only .jpg, .jpeg, .png, .webp, and .svg files are allowed.");
        });
    }
}
