using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.EndPoints.WebApi.HATEOAS;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.HATEOAS;

public class CategoryLinksTests
{
    private readonly IUrlHelper _urlHelper;

    public CategoryLinksTests()
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
    public void Should_Return_Allowed_Links_For_Root_Category()
    {
        // Arrange
        const long categoryId = 1;

        var category = CreateCategory(
            categoryId,
            null,
            new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            });

        // Act
        var links = CategoryLinks.ForDetails(_urlHelper, category);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "changeTitle",
            "changeParent",
            "remove");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Categories.GetById,
            categoryId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Categories.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "changeTitle",
            ApiRouteNames.Categories.ChangeTitle,
            categoryId,
            "PUT");

        AssertLink(
            links,
            "changeParent",
            ApiRouteNames.Categories.ChangeParent,
            categoryId,
            "PUT");

        AssertLink(
            links,
            "remove",
            ApiRouteNames.Categories.Remove,
            categoryId,
            "DELETE");
    }

    [Fact]
    public void Should_Return_Allowed_Links_For_Child_Category()
    {
        // Arrange
        const long categoryId = 2;
        const long parentId = 1;

        var category = CreateCategory(
            categoryId,
            parentId,
            new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            });

        // Act
        var links = CategoryLinks.ForDetails(_urlHelper, category);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "changeTitle",
            "changeParent",
            "getParent",
            "remove");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Categories.GetById,
            categoryId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Categories.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "changeTitle",
            ApiRouteNames.Categories.ChangeTitle,
            categoryId,
            "PUT");

        AssertLink(
            links,
            "changeParent",
            ApiRouteNames.Categories.ChangeParent,
            categoryId,
            "PUT");

        AssertLink(
            links,
            "getParent",
            ApiRouteNames.Categories.GetParentByChildId,
            categoryId,
            "GET");

        AssertLink(
            links,
            "remove",
            ApiRouteNames.Categories.Remove,
            categoryId,
            "DELETE");
    }

    [Fact]
    public void Should_Return_Allowed_Links_For_Removed_Category()
    {
        // Arrange
        const long categoryId = 3;
        const long parentId = 1;

        var category = CreateCategory(
            categoryId,
            parentId,
            new CategoryActionsViewModel
            {
                CanRemove = false,
                CanRestore = true
            });

        // Act
        var links = CategoryLinks.ForDetails(_urlHelper, category);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "changeTitle",
            "changeParent",
            "getParent",
            "restore");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Categories.GetById,
            categoryId,
            "GET");

        AssertLink(
            links,
            "collection",
            ApiRouteNames.Categories.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "changeTitle",
            ApiRouteNames.Categories.ChangeTitle,
            categoryId,
            "PUT");

        AssertLink(
            links,
            "changeParent",
            ApiRouteNames.Categories.ChangeParent,
            categoryId,
            "PUT");

        AssertLink(
            links,
            "getParent",
            ApiRouteNames.Categories.GetParentByChildId,
            categoryId,
            "GET");

        AssertLink(
            links,
            "restore",
            ApiRouteNames.Categories.Restore,
            categoryId,
            "POST");
    }

    [Fact]
    public void Should_Not_Return_Remove_Or_Restore_Links_When_Both_Actions_Are_Not_Allowed()
    {
        // Arrange
        const long categoryId = 4;
        const long parentId = 1;

        var category = CreateCategory(
            categoryId,
            parentId,
            new CategoryActionsViewModel
            {
                CanRemove = false,
                CanRestore = false
            });

        // Act
        var links = CategoryLinks.ForDetails(_urlHelper, category);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "collection",
            "changeTitle",
            "changeParent",
            "getParent");

        links.Should().NotContainKey("remove");
        links.Should().NotContainKey("restore");
    }

    [Fact]
    public void Should_Return_Collection_Links()
    {
        // Act
        var links = CategoryLinks.ForCollection(_urlHelper);

        // Assert
        links.Keys.Should().BeEquivalentTo(
            "self",
            "create");

        AssertLink(
            links,
            "self",
            ApiRouteNames.Categories.GetAll,
            null,
            "GET");

        AssertLink(
            links,
            "create",
            ApiRouteNames.Categories.Create,
            null,
            "POST");
    }

    private static CategoryViewModel CreateCategory(
        long id,
        long? parentId,
        CategoryActionsViewModel actions)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = $"Category {id}",
            ParentId = parentId,
            IsRemoved = !actions.CanRemove,
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