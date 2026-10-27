using FluentValidation;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.MoveToDraft;

public class MoveToDraftArticleCommandValidator
    : AbstractValidator<MoveToDraftArticleCommand>
{
    public MoveToDraftArticleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Article ID must be greater than zero.");
    }
}