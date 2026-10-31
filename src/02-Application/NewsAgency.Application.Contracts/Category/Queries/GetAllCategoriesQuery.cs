using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Queries;

public record GetAllCategoriesQuery : IQuery<OperationResult<List<CategoryViewModel>>>;