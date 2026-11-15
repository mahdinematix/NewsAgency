using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;

namespace NewsAgency.EndPoints.WebApi.Extensions;

public static class AuthorResultExtensions
{
    public static IActionResult ToAuthorResult(
        this ControllerBase controller,
        OperationResult<AuthorViewModel> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        var links = AuthorLinks.ForDetails(
            controller.Url,
            result.Data);

        return controller.ToActionResult(result, links);
    }

    public static IActionResult ToAuthorCollectionResult(
        this ControllerBase controller,
        OperationResult<List<AuthorViewModel>> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        var authors = result.Data
            .Select(author => new ApiResource<AuthorViewModel>
            {
                Data = author,
                Links = AuthorLinks.ForDetails(controller.Url, author)
            })
            .ToList();

        var response = new ApiResponse<List<ApiResource<AuthorViewModel>>>
        {
            Data = authors,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = AuthorLinks.ForCollection(controller.Url)
        };

        return controller.Ok(response);
    }
}