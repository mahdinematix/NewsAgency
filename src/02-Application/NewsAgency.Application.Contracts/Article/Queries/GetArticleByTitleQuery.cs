using NewsAgency.Application.Contracts.Common;

namespace NewsAgency.Application.Contracts.Article.Queries;

public record GetArticleByTitleQuery(string Title) : IQuery<OperationResult<ArticleViewModel>>;