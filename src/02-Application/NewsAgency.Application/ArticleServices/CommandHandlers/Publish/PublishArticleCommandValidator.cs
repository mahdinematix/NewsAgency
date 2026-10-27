using FluentValidation;
using NewsAgency.Application.Contracts.Article.Commands;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Publish;

public class PublishArticleCommandValidator
    : AbstractValidator<PublishArticleCommand>
{
    public PublishArticleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage("Article ID must be greater than zero.");
    }
}