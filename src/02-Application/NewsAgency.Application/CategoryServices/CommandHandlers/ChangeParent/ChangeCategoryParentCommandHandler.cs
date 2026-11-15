using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

namespace NewsAgency.Application.CategoryServices.CommandHandlers.ChangeParent;

public class ChangeCategoryParentCommandHandler
    : ICommandHandler<ChangeCategoryParentCommand, OperationResult<CategoryViewModel>>
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;

    public ChangeCategoryParentCommandHandler(
        ICategoryCommandRepository commandRepository,
        ICategoryQueryRepository queryRepository)
    {
        _commandRepository = commandRepository;
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<CategoryViewModel>> Handle(
        ChangeCategoryParentCommand request,
        CancellationToken cancellationToken)
    {
        var operation = new OperationResult<CategoryViewModel>();

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

        var parentIsRemoved = false;

        if (request.ParentId.HasValue)
        {
            var parent = await _queryRepository.GetByAsync(
                request.ParentId.Value,
                cancellationToken);

            if (parent is null)
            {
                return operation.Failed(
                    OperationErrorCode.NotFound,
                    ApplicationMessages.RecordNotFound);
            }

            parentIsRemoved = parent.IsRemoved;
        }

        category.ChangeParent(request.ParentId);

        await _commandRepository.SaveChangesAsync(cancellationToken);

        var hasActiveChildren = await _queryRepository.HasActiveChildrenAsync(
            category.Id,
            cancellationToken);

        return operation.Succeeded(new CategoryViewModel
        {
            Id = category.Id,
            Title = category.Title.Value,
            IsRemoved = category.IsRemoved,
            ParentId = category.ParentId,
            Actions = new CategoryActionsViewModel
            {
                CanRemove = category.CanRemove(hasActiveChildren),
                CanRestore = category.CanRestore(parentIsRemoved)
            }
        });
    }
}