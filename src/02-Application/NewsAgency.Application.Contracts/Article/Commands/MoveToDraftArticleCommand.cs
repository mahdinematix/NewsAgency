using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Commands;

public record MoveToDraftArticleCommand(long Id) : IRequest<OperationResult>;