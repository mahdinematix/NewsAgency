using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Commands;

public record ChangeCategoryParentCommand(long Id, long? ParentId) : ICommand<OperationResult<CategoryViewModel>>;