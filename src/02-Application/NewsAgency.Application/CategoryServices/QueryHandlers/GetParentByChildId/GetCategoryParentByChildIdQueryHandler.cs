using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetParentByChildId;

public class GetCategoryParentByChildIdQueryHandler
    : IQueryHandler<GetCategoryParentByChildIdQuery, OperationResult<CategoryViewModel?>>
{
    private readonly ICategoryQueryRepository _queryRepository;

    public GetCategoryParentByChildIdQueryHandler(
        ICategoryQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<CategoryViewModel?>> Handle(
        GetCategoryParentByChildIdQuery request,
        CancellationToken cancellationToken)
    {
        var operation = new OperationResult<CategoryViewModel?>();

        if (request is null)
        {
            return operation.Failed(
                OperationErrorCode.Validation,
                ApplicationMessages.ValueMustNotBeNull);
        }

        var child = await _queryRepository.GetByAsync(
            request.ChildId,
            cancellationToken);

        if (child is null)
        {
            return operation.Failed(
                OperationErrorCode.NotFound,
                ApplicationMessages.RecordNotFound);
        }

        var parent = await _queryRepository.GetParentByAsync(
            request.ChildId,
            cancellationToken);

        if (parent is null)
        {
            return operation.Succeeded(null);
        }

        var parentIsRemoved = false;

        if (parent.ParentId.HasValue)
        {
            var grandparent = await _queryRepository.GetByAsync(
                parent.ParentId.Value,
                cancellationToken);

            if (grandparent is null)
            {
                return operation.Failed(
                    OperationErrorCode.NotFound,
                    ApplicationMessages.RecordNotFound);
            }

            parentIsRemoved = grandparent.IsRemoved;
        }

        var hasActiveChildren = await _queryRepository.HasActiveChildrenAsync(
            parent.Id,
            cancellationToken);

        return operation.Succeeded(new CategoryViewModel
        {
            Id = parent.Id,
            Title = parent.Title.Value,
            IsRemoved = parent.IsRemoved,
            ParentId = parent.ParentId,
            Actions = new CategoryActionsViewModel
            {
                CanRemove = parent.CanRemove(hasActiveChildren),
                CanRestore = parent.CanRestore(parentIsRemoved)
            }
        });
    }
}