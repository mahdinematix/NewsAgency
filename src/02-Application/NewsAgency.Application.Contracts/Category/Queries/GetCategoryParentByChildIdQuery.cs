using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Queries;

public record GetCategoryParentByChildIdQuery(long ChildId) : IQuery<OperationResult<CategoryViewModel?>>;