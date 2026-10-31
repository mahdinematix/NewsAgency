using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetByTitle;

public class GetCategoryByTitleQueryHandler : IQueryHandler<GetCategoryByTitleQuery, OperationResult<CategoryViewModel>>
{
    private readonly ICategoryQueryRepository _queryRepository;

    public GetCategoryByTitleQueryHandler(ICategoryQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<CategoryViewModel>> Handle(GetCategoryByTitleQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<CategoryViewModel>();

        if (request is null)
        {
            return operation.Failed(OperationErrorCode.Validation, ApplicationMessages.ValueMustNotBeNull);
        }

        var category = await _queryRepository.GetByAsync(request.Title, cancellationToken);

        if (category is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(new CategoryViewModel
        {
            Title = category.Title.Value,
            ParentId = category.ParentId,
            Id = category.Id,
            IsRemoved = category.IsRemoved
        });
    }
}