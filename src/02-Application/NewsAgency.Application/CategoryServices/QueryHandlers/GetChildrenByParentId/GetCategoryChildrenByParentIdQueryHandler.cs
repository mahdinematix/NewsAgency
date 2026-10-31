using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

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

        return operation.Succeeded(children.Select(x => new CategoryViewModel
        {
            Id = x.Id,
            Title = x.Title.Value,
            IsRemoved = x.IsRemoved,
            ParentId = x.ParentId
        }).ToList());
    }
}

