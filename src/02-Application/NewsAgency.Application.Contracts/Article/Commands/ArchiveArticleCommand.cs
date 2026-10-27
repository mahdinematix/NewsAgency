using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Commands;

public record ArchiveArticleCommand(long Id) : ICommand<OperationResult>;