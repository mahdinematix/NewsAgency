using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Commands;

public record CreateArticleCommand(
    long Id,
    string Title,
    string Content,
    long AuthorId,
    long CategoryId)
    : IRequest<OperationResult>;

