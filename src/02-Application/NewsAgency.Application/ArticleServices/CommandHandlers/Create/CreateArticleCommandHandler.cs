using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Create;

public class CreateArticleCommandHandler : ICommandHandler<CreateArticleCommand, OperationResult<long>>
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    public CreateArticleCommandHandler(IArticleCommandRepository commandRepository, IArticleQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<long>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<long>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            if (await _queryRepository.Exists(x => x.Title.Value == request.Title, cancellationToken))
            {
                return operation.Failed(OperationErrorCode.Conflict, ApplicationMessages.ValueCannotBeDuplicated);
            }
            var article = new Article(new ArticleTitle(request.Title), new ArticleContent(request.Content), request.AuthorId, request.CategoryId);
            await _commandRepository.CreateAsync(article, cancellationToken);
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded(article.Id);
        }
        catch (InvalidValueObjectStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }

}