using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.Publish;

public class PublishArticleCommandHandler : IRequestHandler<PublishArticleCommand, OperationResult>
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    public PublishArticleCommandHandler(IArticleCommandRepository commandRepository, IArticleQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(PublishArticleCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        try
        {
            var article = await _queryRepository.GetByAsync(request.Id, cancellationToken);

            if (article is null)
            {
                return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
            }

            article.Publish();
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (DomainStateException ex)
        {
            return operation.Failed(OperationErrorCode.LogicError, ex.Message);
        }
    }
}