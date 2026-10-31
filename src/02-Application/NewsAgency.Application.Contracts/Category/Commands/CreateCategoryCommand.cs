using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Commands;

public record CreateCategoryCommand(string Title, long? ParentId) : ICommand<OperationResult<long>>;