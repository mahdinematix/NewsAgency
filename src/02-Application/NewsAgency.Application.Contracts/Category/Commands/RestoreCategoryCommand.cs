using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Commands;

public record RestoreCategoryCommand(long Id) : ICommand<OperationResult<CategoryViewModel>>;