using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Category.Commands;

public record ChangeCategoryTitleCommand(long Id, string Title) : ICommand<OperationResult>;