using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Restore;

public class RestoreCategoryCommandHandler
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;

    public RestoreCategoryCommandHandler(ICategoryCommandRepository commandRepository, ICategoryQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(RestoreCategoryCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult();

        if (request is null)
        {
            return operation.Failed(
                OperationErrorCode.Validation,
                ApplicationMessages.ValueMustNotBeNull);
        }

        var category = await _queryRepository.GetByAsync(
            request.Id,
            cancellationToken);

        if (category is null)
        {
            return operation.Failed(
                OperationErrorCode.NotFound,
                ApplicationMessages.RecordNotFound);
        }

        if (!category.IsRemoved)
        {
            return operation.Succeeded();
        }

        var parentIsRemoved = await _queryRepository.IsParentRemovedAsync(
            category.ParentId,
            cancellationToken);

        try
        {
            category.Restore(parentIsRemoved);
        }
        catch (DomainStateException ex)
        {
            return operation.Failed(
                OperationErrorCode.Conflict,
                ex.Message);
        }

        await _commandRepository.SaveChangesAsync(cancellationToken);

        return operation.Succeeded();
    }
}