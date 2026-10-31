using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

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

        return operation.Succeeded(new CategoryViewModel
        {
            Id = category.Id,
            Title = category.Title.Value,
            IsRemoved = category.IsRemoved,
            ParentId = category.ParentId
        });

    }
}