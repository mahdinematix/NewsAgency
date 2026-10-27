using FluentValidation;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Reject;

public class RejectArticleCommandValidator
    : AbstractValidator<RejectArticleCommand>
{
    public RejectArticleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Article ID must be greater than zero.");
    }
}