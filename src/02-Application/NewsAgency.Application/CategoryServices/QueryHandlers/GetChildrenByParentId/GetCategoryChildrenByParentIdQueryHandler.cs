using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Utilities;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetChildrenByParentId;

public class GetCategoryChildrenByParentIdQueryHandler : IQueryHandler<GetCategoryChildrenByParentIdQuery, OperationResult<List<CategoryViewModel>>>
{
    private readonly ICategoryQueryRepository _queryRepository;

    public GetCategoryChildrenByParentIdQueryHandler(ICategoryQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<List<CategoryViewModel>>> Handle(GetCategoryChildrenByParentIdQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<List<CategoryViewModel>>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var children = await _queryRepository.GetChildrenByAsync(request.ParentId, cancellationToken);

        if (children is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }
        var categoryViewModels = new List<CategoryViewModel>();

        foreach (var category in children)
        {
            var parentIsRemoved = false;

            if (category.ParentId.HasValue)
            {
                var parent = await _queryRepository.GetByAsync(
                    category.ParentId.Value,
                    cancellationToken);

                if (parent is null)
                {
                    return operation.Failed(
                        OperationErrorCode.NotFound,
                        ApplicationMessages.RecordNotFound);
                }

                parentIsRemoved = parent.IsRemoved;
            }

            var hasActiveChildren = await _queryRepository.HasActiveChildrenAsync(
                category.Id,
                cancellationToken);

            categoryViewModels.Add(new CategoryViewModel
            {
                Id = category.Id,
                Title = category.Title.Value,
                ParentId = category.ParentId,
                IsRemoved = category.IsRemoved,
                CreationDate = category.CreationDate.ToFarsi(),
                Actions = new CategoryActionsViewModel
                {
                    CanRemove = category.CanRemove(hasActiveChildren),
                    CanRestore = category.CanRestore(parentIsRemoved)
                }
            });
        }

        return operation.Succeeded(categoryViewModels);
    }
}