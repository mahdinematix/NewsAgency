using MediatR;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.ArticleServices.CommandHandlers.MoveToDraft;

public class MoveToDraftArticleCommandHandler : IRequestHandler<MoveToDraftArticleCommand, OperationResult>
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    public MoveToDraftArticleCommandHandler(IArticleCommandRepository commandRepository, IArticleQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(MoveToDraftArticleCommand request, CancellationToken cancellationToken)
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

            article.MoveToDraft();
            await _commandRepository.SaveChangesAsync(cancellationToken);
            return operation.Succeeded();
        }
        catch (DomainStateException ex)
        {
            return operation.Failed(OperationErrorCode.Validation, ex.Message);
        }
    }
}