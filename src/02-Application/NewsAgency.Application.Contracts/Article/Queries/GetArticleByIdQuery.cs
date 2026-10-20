using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Queries;

public record GetArticleByIdQuery(long Id) : IRequest<OperationResult<ArticleViewModel>>;