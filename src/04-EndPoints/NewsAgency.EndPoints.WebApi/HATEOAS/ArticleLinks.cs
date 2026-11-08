using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.HATEOAS;

public static class ArticleLinks
{
    public static Dictionary<string, ApiLink> ForDetails(
        IUrlHelper url,
        ArticleViewModel article)
    {
        var id = article.Id;

        var links = new Dictionary<string, ApiLink>
        {
            ["self"] = new(
                url.Link(ApiRouteNames.Articles.GetById, new { id })!,
                "GET"),

            ["collection"] = new(
                url.Link(ApiRouteNames.Articles.GetAll, null)!,
                "GET")
        };

        if (article.Actions.CanEdit)
        {
            links["edit"] = new(
                url.Link(ApiRouteNames.Articles.Edit, new { id })!,
                "PUT");
        }

        if (article.Actions.CanPublish)
        {
            links["publish"] = new(
                url.Link(ApiRouteNames.Articles.Publish, new { id })!,
                "POST");
        }

        if (article.Actions.CanReject)
        {
            links["reject"] = new(
                url.Link(ApiRouteNames.Articles.Reject, new { id })!,
                "POST");
        }

        if (article.Actions.CanArchive)
        {
            links["archive"] = new(
                url.Link(ApiRouteNames.Articles.Archive, new { id })!,
                "POST");
        }

        if (article.Actions.CanMoveToDraft)
        {
            links["moveToDraft"] = new(
                url.Link(ApiRouteNames.Articles.MoveToDraft, new { id })!,
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
                url.Link(ApiRouteNames.Articles.GetAll, null)!,
                "GET"),

            ["create"] = new(
                url.Link(ApiRouteNames.Articles.Create, null)!,
                "POST")
        };
    }
}