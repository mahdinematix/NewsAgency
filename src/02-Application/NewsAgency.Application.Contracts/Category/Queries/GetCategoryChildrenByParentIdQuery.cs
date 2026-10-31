using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Queries;

public record GetCategoryChildrenByParentIdQuery(long ParentId) : IQuery<OperationResult<List<CategoryViewModel>>>;