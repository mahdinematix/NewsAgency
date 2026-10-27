using FluentValidation;
using NewsAgency.Application.Contracts.Article.Queries;

namespace NewsAgency.Application.ArticleServices.QueryHandlers.GetById;

public class GetArticleByIdQueryValidator
    : AbstractValidator<GetArticleByIdQuery>
{
    public GetArticleByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage("Article ID must be greater than zero.");
    }
}