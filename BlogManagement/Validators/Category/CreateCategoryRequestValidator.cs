using BlogManagement.DTO.Category;
using FluentValidation;

namespace BlogManagement.Validators.Category;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequestDTO>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(ct => ct.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(3)
                .WithMessage("Name must be at least 3 characters long.")
                .MaximumLength(55)
                .WithMessage("Name must be at least 3 characters long.");

        RuleFor(ct => ct.Description)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MinimumLength(10)
                .WithMessage("Name must be at least 10 characters long.")
                .MaximumLength(255)
                .WithMessage("Name must be at least 255 characters long.");

        RuleFor(x => x.Icon)
                .Must(file => file == null || new[] { ".jpg", ".jpeg", ".png" }
                .Contains(Path.GetExtension(file.FileName), StringComparer.OrdinalIgnoreCase))
                .WithMessage("Only .jpg, .jpeg, and .png files are allowed.");

    }
}
