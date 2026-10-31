using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Queries;

public record GetCategoryByTitleQuery(string Title) : IQuery<OperationResult<CategoryViewModel>>;