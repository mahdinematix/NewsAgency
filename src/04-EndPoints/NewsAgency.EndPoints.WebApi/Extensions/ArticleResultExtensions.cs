using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.HATEOAS;

namespace NewsAgency.EndPoints.WebApi.Extensions;

public static class ArticleResultExtensions
{
    public static IActionResult ToArticleResult(
        this ControllerBase controller,
        OperationResult<ArticleViewModel> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        var links = ArticleLinks.ForDetails(
            controller.Url,
            result.Data);

        return controller.ToActionResult(result, links);
    }
}
