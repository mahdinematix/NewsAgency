using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;

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

    public static IActionResult ToArticleCollectionResult(
        this ControllerBase controller,
        OperationResult<List<ArticleViewModel>> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        var articles = result.Data
            .Select(article => new ApiResource<ArticleViewModel>
            {
                Data = article,
                Links = ArticleLinks.ForDetails(controller.Url, article)
            })
            .ToList();

        var response = new ApiResponse<List<ApiResource<ArticleViewModel>>>
        {
            Data = articles,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = ArticleLinks.ForCollection(controller.Url)
        };

        return controller.Ok(response);
    }
}