using FluentValidation;
using NewsAgency.Application.Contracts.Article.Queries;

namespace NewsAgency.Application.ArticleServices.QueryHandlers.GetByTitle;

public class GetArticleByTitleQueryValidator
    : AbstractValidator<GetArticleByTitleQuery>
{
    public GetArticleByTitleQueryValidator()
    {
        RuleFor(query => query.Title)
            .Must(title => !string.IsNullOrWhiteSpace(title))
            .WithMessage("Article title is required.")
            .MinimumLength(2)
            .WithMessage("Article title must be at least 2 characters long.")
            .MaximumLength(50)
            .WithMessage("Article title cannot exceed 50 characters.");
    }
}