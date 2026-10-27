using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Commands;

public record MoveToDraftArticleCommand(long Id) : ICommand<OperationResult>;