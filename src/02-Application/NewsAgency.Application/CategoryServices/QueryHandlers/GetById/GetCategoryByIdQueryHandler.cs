using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Utilities;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetById;

public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, OperationResult<CategoryViewModel>>
{
    private readonly ICategoryQueryRepository _queryRepository;

    public GetCategoryByIdQueryHandler(ICategoryQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<CategoryViewModel>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<CategoryViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var category = await _queryRepository.GetByAsync(request.Id, cancellationToken);

        if (category is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        var parentIsRemoved = false;

        if (category.ParentId.HasValue)
        {
            var parent = await _queryRepository.GetByAsync(
                category.ParentId.Value,
                cancellationToken);

            if (parent is null)
            {
                return operation.Failed(
                    OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
            }

            parentIsRemoved = parent.IsRemoved;
        }

        var hasActiveChildren = await _queryRepository.HasActiveChildrenAsync(
            category.Id,
            cancellationToken);

        return operation.Succeeded(new CategoryViewModel
        {
            Id = category.Id,
            Title = category.Title.Value,
            IsRemoved = category.IsRemoved,
            ParentId = category.ParentId,
            CreationDate = category.CreationDate.ToFarsi(),
            Actions = new CategoryActionsViewModel
            {
            CanRemove = category.CanRemove(hasActiveChildren),
            CanRestore = category.CanRestore(parentIsRemoved)
        }
        });

    }
}