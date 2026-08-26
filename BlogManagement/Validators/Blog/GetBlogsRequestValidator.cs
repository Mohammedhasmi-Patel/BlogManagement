using BlogManagement.DTO.Blog;
using FluentValidation;

namespace BlogManagement.Validators.Blog;

public class GetBlogsRequestValidator : AbstractValidator<GetBlogsRequestDTO>
{
    private static readonly string[] AllowedSortBy = ["createdat", "title", "viewcount", "publishedat", "readingtimeminutes"];
    private static readonly string[] AllowedSortOrders = ["asc", "desc"];

    public GetBlogsRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50)
            .WithMessage("Page size must be between 1 and 50.");

        When(x => !string.IsNullOrWhiteSpace(x.SortBy), () =>
        {
            RuleFor(x => x.SortBy!)
                .Must(sortBy => AllowedSortBy.Contains(sortBy.Trim().ToLower()))
                .WithMessage($"SortBy must be one of the following: {string.Join(", ", AllowedSortBy)}.");
        });

        When(x => !string.IsNullOrWhiteSpace(x.SortOrder), () =>
        {
            RuleFor(x => x.SortOrder!)
                .Must(sortOrder => AllowedSortOrders.Contains(sortOrder.Trim().ToLower()))
                .WithMessage("SortOrder must be either 'asc' or 'desc'.");
        });
    }
}
