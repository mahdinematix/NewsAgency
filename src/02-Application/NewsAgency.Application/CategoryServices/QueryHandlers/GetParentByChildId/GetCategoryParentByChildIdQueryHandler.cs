using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetParentByChildId;

public class GetCategoryParentByChildIdQueryHandler : IQueryHandler<GetCategoryParentByChildIdQuery, OperationResult<CategoryViewModel?>>
{
    private readonly ICategoryQueryRepository _queryRepository;

    public GetCategoryParentByChildIdQueryHandler(ICategoryQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<CategoryViewModel?>> Handle(GetCategoryParentByChildIdQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<CategoryViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var child = await _queryRepository.GetByAsync(request.ChildId, cancellationToken);

        if (child is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        var parent = await _queryRepository.GetParentByAsync(request.ChildId, cancellationToken);

        return operation.Succeeded(parent is null ? null : new CategoryViewModel
        {
            Id = parent.Id,
            Title = parent.Title.Value,
            IsRemoved = parent.IsRemoved,
            ParentId = parent.ParentId
        });
    }
}