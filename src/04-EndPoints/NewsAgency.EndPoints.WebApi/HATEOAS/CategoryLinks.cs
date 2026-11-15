using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.HATEOAS;

public static class CategoryLinks
{
    public static Dictionary<string, ApiLink> ForDetails(
        IUrlHelper url,
        CategoryViewModel category)
    {
        var id = category.Id;

        var links = new Dictionary<string, ApiLink>
        {
            ["self"] = new(
                url.Link(ApiRouteNames.Categories.GetById, new { id })!,
                "GET"),

            ["collection"] = new(
                url.Link(ApiRouteNames.Categories.GetAll, null)!,
                "GET")
        };

        links["changeTitle"] = new(
            url.Link(ApiRouteNames.Categories.ChangeTitle, new { id })!,
            "PUT");

        links["changeParent"] = new(
            url.Link(ApiRouteNames.Categories.ChangeParent, new { id })!,
            "PUT");

        if (category.ParentId.HasValue)
        {
            links["getParent"] = new(
                url.Link(
                    ApiRouteNames.Categories.GetParentByChildId,
                    new { id })!,
                "GET");
        }

        if (category.Actions.CanRemove)
        {
            links["remove"] = new(
                url.Link(ApiRouteNames.Categories.Remove, new { id })!,
                "DELETE");
        }

        if (category.Actions.CanRestore)
        {
            links["restore"] = new(
                url.Link(ApiRouteNames.Categories.Restore, new { id })!,
                "POST");
        }

        return links;
    }

    public static Dictionary<string, ApiLink> ForCollection(
        IUrlHelper url)
    {
        return new Dictionary<string, ApiLink>
        {
            ["self"] = new(
                url.Link(ApiRouteNames.Categories.GetAll, null)!,
                "GET"),

            ["create"] = new(
                url.Link(ApiRouteNames.Categories.Create, null)!,
                "POST")
        };
    }
}