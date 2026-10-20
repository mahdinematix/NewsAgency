using MediatR;
using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Queries;

public record GetAllArticlesQuery : IRequest<OperationResult<List<ArticleViewModel>>>;