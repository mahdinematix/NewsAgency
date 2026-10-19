using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers;

public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, OperationResult>
{
    private readonly IArticleCommandRepository _commandRepository;

    public CreateArticleCommandHandler(IArticleCommandRepository commandRepository)
    {
        _commandRepository = commandRepository;
    }

    public async Task<OperationResult> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult();
        try
        {
            if (request is null)
            {
                return operation.Failed(ApplicationMessages.ValueMustNotBeNull);
            }

            var article = new Article(new ArticleTitle(request.Title), new ArticleContent(request.Content), request.AuthorId, request.CategoryId);
            await _commandRepository.CreateAsync(article, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(ex.Message);
        }
    }

}


