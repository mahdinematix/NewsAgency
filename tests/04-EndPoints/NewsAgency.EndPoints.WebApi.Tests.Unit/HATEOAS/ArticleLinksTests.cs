using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.HATEOAS;

public class ArticleLinksTests
{
    private readonly IUrlHelper _urlHelper;

    public ArticleLinksTests()
    {
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper.Link(
                Arg.Any<string>(),
                Arg.Any<object?>())
            .Returns(callInfo =>
            {
                var routeName = callInfo.ArgAt<string>(0);
                var routeValues = callInfo.ArgAt<object?>(1);

                var id = routeValues?
                    .GetType()
                    .GetProperty("id")?
                    .GetValue(routeValues);

                return id is null
                    ? $"/test/{routeName}"
                    : $"/test/{routeName}/{id}";
            });
    }

    [Fact]
    public void Should_Return_Allowed_Links_For_Draft_Article()
    {
        // Arrange
        const long articleId = 1;

        var article = CreateArticle(
            articleId,
            "Draft",
            new ArticleActionsViewModel
            {
                CanEdit = true,
                CanPublish = true,
                CanReject = true,
                CanArchive = false,
                CanMoveToDraft = false
            });

        // Act
        var links = ArticleLinks.ForDetails(_urlHelper, article);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "edit",
            "publish",
            "reject");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Articles.GetById,
            articleId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Articles.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "edit",
            ApiRouteNames.Articles.Edit,
            articleId,
            "PUT");

        AssertLink(
            links,
            "publish",
            ApiRouteNames.Articles.Publish,
            articleId,
            "POST");

        AssertLink(
            links,
            "reject",
            ApiRouteNames.Articles.Reject,
            articleId,
            "POST");
    }


    [Fact]
    public void Should_Return_Allowed_Links_For_Published_Article()
    {
        // Arrange
        const long articleId = 2;

        var article = CreateArticle(
            articleId,
            "Published",
            new ArticleActionsViewModel
            {
                CanEdit = false,
                CanPublish = false,
                CanReject = false,
                CanArchive = true,
                CanMoveToDraft = false
            });

        // Act
        var links = ArticleLinks.ForDetails(_urlHelper, article);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "archive");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Articles.GetById,
            articleId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Articles.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "archive",
            ApiRouteNames.Articles.Archive,
            articleId,
            "POST");
    }


    [Fact]
    public void Should_Return_Allowed_Links_For_Rejected_Article()
    {
        // Arrange
        const long articleId = 3;

        var article = CreateArticle(
            articleId,
            "Rejected",
            new ArticleActionsViewModel
            {
                CanEdit = true,
                CanPublish = false,
                CanReject = false,
                CanArchive = false,
                CanMoveToDraft = true
            });

        // Act
        var links = ArticleLinks.ForDetails(_urlHelper, article);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "edit",
            "moveToDraft");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Articles.GetById,
            articleId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Articles.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "edit",
            ApiRouteNames.Articles.Edit,
            articleId,
            "PUT");

        AssertLink(
            links,
            "moveToDraft",
            ApiRouteNames.Articles.MoveToDraft,
            articleId,
            "POST");
    }

    [Fact]
    public void Should_Return_Allowed_Links_For_Archived_Article()
    {
        // Arrange
        const long articleId = 4;

        var article = CreateArticle(
            articleId,
            "Archived",
            new ArticleActionsViewModel
            {
                CanEdit = false,
                CanPublish = false,
                CanReject = false,
                CanArchive = false,
                CanMoveToDraft = true
            });

        // Act
        var links = ArticleLinks.ForDetails(_urlHelper, article);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "moveToDraft");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Articles.GetById,
            articleId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Articles.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "moveToDraft",
            ApiRouteNames.Articles.MoveToDraft,
            articleId,
            "POST");
    }

    [Fact]
    public void Should_Return_Collection_Links()
    {
        // Act
        var links = ArticleLinks.ForCollection(_urlHelper);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "create");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Articles.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "create",
            ApiRouteNames.Articles.Create,
            null,
            "POST");
    }


    private static ArticleViewModel CreateArticle(
        long id,
        string status,
        ArticleActionsViewModel actions)
    {
        return new ArticleViewModel
        {
            Id = id,
            Title = $"Article {id}",
            Content = $"Article {id} content",
            AuthorId = 1,
            CategoryId = 1,
            Status = status,
            Actions = actions
        };
    }

    private static void AssertLink(
        Dictionary<string, ApiLink> links,
        string relation,
        string routeName,
        long? id,
        string expectedMethod)
    {
        var link = links[relation];

        var expectedHref = id.HasValue
            ? $"/test/{routeName}/{id}"
            : $"/test/{routeName}";

        link.Href.Should().Be(expectedHref);
        link.Method.Should().Be(expectedMethod);
    }
}