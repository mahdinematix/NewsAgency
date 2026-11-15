using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;

namespace NewsAgency.EndPoints.WebApi.HATEOAS;

public static class AuthorLinks
{
    public static Dictionary<string, ApiLink> ForDetails(
        IUrlHelper url,
        AuthorViewModel author)
    {
        return ForDetails(url, author.Id);
    }

    public static Dictionary<string, ApiLink> ForDetails(
        IUrlHelper url,
        long id)
    {
        return new Dictionary<string, ApiLink>
        {
            ["self"] = new(
                url.Link(ApiRouteNames.Authors.GetById, new { id })!,
                "GET"),

            ["collection"] = new(
                url.Link(ApiRouteNames.Authors.GetAll, null)!,
                "GET"),

            ["changeFirstName"] = new(
                url.Link(ApiRouteNames.Authors.ChangeFirstName, new { id })!,
                "PUT"),

            ["changeLastName"] = new(
                url.Link(ApiRouteNames.Authors.ChangeLastName, new { id })!,
                "PUT"),

            ["changeEmail"] = new(
                url.Link(ApiRouteNames.Authors.ChangeEmail, new { id })!,
                "PUT"),

            ["changePhoneNumber"] = new(
                url.Link(ApiRouteNames.Authors.ChangePhoneNumber, new { id })!,
                "PUT")
        };
    }

    public static Dictionary<string, ApiLink> ForCollection(
        IUrlHelper url)
    {
        return new Dictionary<string, ApiLink>
        {
            ["self"] = new(
                url.Link(ApiRouteNames.Authors.GetAll, null)!,
                "GET"),

            ["create"] = new(
                url.Link(ApiRouteNames.Authors.Create, null)!,
                "POST")
        };
    }
}