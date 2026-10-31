using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.Remove;

public class RemoveCategoryCommandHandler : ICommandHandler<RemoveCategoryCommand, OperationResult>
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;

    public RemoveCategoryCommandHandler(ICategoryCommandRepository commandRepository, ICategoryQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
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

        if (category.IsRemoved)
        {
            return operation.Succeeded();
        }

        var hasActiveChildren =
            await _queryRepository.HasActiveChildrenAsync(
                category.Id,
                cancellationToken);

        try
        {
            category.Remove(hasActiveChildren);
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
