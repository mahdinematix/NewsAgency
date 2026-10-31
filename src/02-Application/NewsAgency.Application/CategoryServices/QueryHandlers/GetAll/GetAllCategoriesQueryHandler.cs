using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;

namespace NewsAgency.Application.CategoryServices.QueryHandlers.GetAll;

public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, OperationResult<List<CategoryViewModel>>>
{
    private readonly ICategoryQueryRepository _queryRepository;

    public GetAllCategoriesQueryHandler(ICategoryQueryRepository queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<OperationResult<List<CategoryViewModel>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var operation = new OperationResult<List<CategoryViewModel>>();

        var categories = await _queryRepository.GetAllAsync(cancellationToken);

        if (categories is null)
        {
            return operation.Failed(OperationErrorCode.NotFound, ApplicationMessages.RecordNotFound);
        }

        return operation.Succeeded(categories.Select(x=> new CategoryViewModel{
            Id = x.Id,
            Title = x.Title.Value,
            ParentId = x.ParentId,
            IsRemoved = x.IsRemoved

        }).ToList());
    }
}