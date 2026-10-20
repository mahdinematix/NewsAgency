using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Commands;

public record RejectArticleCommand(long Id) : IRequest<OperationResult>;