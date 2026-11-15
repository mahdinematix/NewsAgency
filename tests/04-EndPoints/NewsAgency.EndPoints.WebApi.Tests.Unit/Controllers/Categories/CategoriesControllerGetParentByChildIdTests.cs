using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Categories;

public class CategoriesControllerGetParentByChildIdTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerGetParentByChildIdTests()
    {
        _sender = Substitute.For<ISender>();
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper
            .Link(Arg.Any<string>(), Arg.Any<object?>())
            .Returns("/test");

        _controller = new CategoriesController(_sender)
        {
            Url = _urlHelper
        };
    }

    [Fact]
    public async Task Should_Return_Parent_Category_When_Child_Has_Parent()
    {
        // Arrange
        const long childId = 10;

        var parent = CreateCategory(5, "Programming");

        var operation = new OperationResult<CategoryViewModel?>()
            .Succeeded(parent);

        _sender
            .Send(
                Arg.Is<GetCategoryParentByChildIdQuery>(
                    query => query.ChildId == childId),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetParentByChildId(
            childId,
            CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value
            .Should()
            .BeOfType<ApiResponse<CategoryViewModel?>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(parent);
        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryParentByChildIdQuery>(
                query => query.ChildId == childId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Null_When_Child_Has_No_Parent()
    {
        // Arrange
        const long childId = 10;

        var operation = new OperationResult<CategoryViewModel?>()
            .Succeeded(null);

        _sender
            .Send(
                Arg.Is<GetCategoryParentByChildIdQuery>(
                    query => query.ChildId == childId),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetParentByChildId(
            childId,
            CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value
            .Should()
            .BeOfType<ApiResponse<CategoryViewModel?>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeNull();

        response.Links.Should().ContainKey("self");
        response.Links.Should().ContainKey("collection");

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryParentByChildIdQuery>(
                query => query.ChildId == childId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Child_Does_Not_Exist()
    {
        // Arrange
        const long childId = 10;

        var operation = new OperationResult<CategoryViewModel?>()
            .Failed(
                OperationErrorCode.NotFound,
                ApplicationMessages.RecordNotFound);

        _sender
            .Send(
                Arg.Is<GetCategoryParentByChildIdQuery>(
                    query => query.ChildId == childId),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetParentByChildId(
            childId,
            CancellationToken.None);

        // Assert
        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;

        objectResult.StatusCode.Should().Be(404);

        var response = objectResult.Value
            .Should()
            .BeOfType<ApiResponse<CategoryViewModel?>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();

        _urlHelper
            .DidNotReceiveWithAnyArgs()
            .Link(default!, default);

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryParentByChildIdQuery>(
                query => query.ChildId == childId),
            Arg.Any<CancellationToken>());
    }

    private static CategoryViewModel CreateCategory(long id, string title)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = title,
            ParentId = null,
            IsRemoved = false,
            Actions = new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            }
        };
    }
}