using BlogManagement.DTO.Category;
using FluentValidation;

namespace BlogManagement.Validators.Category;

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequestDTO>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Category id is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Category name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Category description is required.");

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
