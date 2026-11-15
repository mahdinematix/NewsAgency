using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.Extensions;

public static class CategoryResultExtensions
{
    public static IActionResult ToCategoryResult(
        this ControllerBase controller,
        OperationResult<CategoryViewModel> result)
    {
        if (!result.IsSuccess)
        {
            return controller.ToActionResult(result);
        }

        var links = CategoryLinks.ForDetails(
            controller.Url,
            result.Data);

        return controller.ToActionResult(result, links);
    }

    public static IActionResult ToCategoryCollectionResult(
        this ControllerBase controller,
        OperationResult<List<CategoryViewModel>> result)
    {
        if (!result.IsSuccess)
            return controller.ToActionResult(result);

        var categories = result.Data
            .Select(category => new ApiResource<CategoryViewModel>
            {
                Data = category,
                Links = CategoryLinks.ForDetails(controller.Url, category)
            })
            .ToList();

        var response = new ApiResponse<List<ApiResource<CategoryViewModel>>>
        {
            Data = categories,
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            ErrorCode = result.ErrorCode,
            Links = CategoryLinks.ForCollection(controller.Url)
        };

        return controller.Ok(response);
    }

    public static IActionResult ToCategoryParentResult(
        this ControllerBase controller,
        OperationResult<CategoryViewModel?> result,
        long childId)
    {
        if (!result.IsSuccess)
            return controller.ToActionResult(result);

        var categoryId = result.Data?.Id ?? childId;

        var routeName = result.Data is null
            ? ApiRouteNames.Categories.GetParentByChildId
            : ApiRouteNames.Categories.GetById;

        var links = new Dictionary<string, ApiLink>
        {
            ["self"] = new(
                controller.Url.Link(routeName, new { id = categoryId })!,
                "GET"),

            ["collection"] = new(
                controller.Url.Link(ApiRouteNames.Categories.GetAll, null)!,
                "GET")
        };

        return controller.ToActionResult(result, links);
    }
}