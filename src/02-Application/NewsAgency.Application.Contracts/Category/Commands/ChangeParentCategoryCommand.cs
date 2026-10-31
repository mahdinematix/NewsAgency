using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Commands;

public record ChangeParentCategoryCommand(long Id, long? ParentId) : ICommand<OperationResult>;