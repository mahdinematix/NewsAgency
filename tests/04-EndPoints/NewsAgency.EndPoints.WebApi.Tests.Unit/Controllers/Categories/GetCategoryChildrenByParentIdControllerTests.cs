using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Categories;

public class CategoriesControllerGetChildrenByParentIdTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerGetChildrenByParentIdTests()
    {
        _sender = Substitute.For<ISender>();
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper.Link(
                Arg.Any<string>(),
                Arg.Any<object?>())
            .Returns("/test");

        _controller = new CategoriesController(_sender)
        {
            Url = _urlHelper
        };
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Category_Children_With_HATEOAS_Links()
    {
        // Arrange
        const long parentId = 10;

        var children = new List<CategoryViewModel>
        {
            CreateCategory(20, "Programming", parentId),
            CreateCategory(30, "Database", parentId)
        };

        var operation = new OperationResult<List<CategoryViewModel>>()
            .Succeeded(children);

        _sender.Send(
                Arg.Any<GetCategoryChildrenByParentIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetChildrenByParentId(
            parentId,
            CancellationToken.None);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<List<ApiResource<CategoryViewModel>>>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().HaveCount(2);

        response.Data.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryChildrenByParentIdQuery>(query =>
                query.ParentId == parentId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Success_Result_When_No_Children_Exist()
    {
        // Arrange
        const long parentId = 10;

        var operation = new OperationResult<List<CategoryViewModel>>()
            .Succeeded(new List<CategoryViewModel>());

        _sender.Send(
                Arg.Any<GetCategoryChildrenByParentIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetChildrenByParentId(
            parentId,
            CancellationToken.None);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<List<ApiResource<CategoryViewModel>>>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().BeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryChildrenByParentIdQuery>(query =>
                query.ParentId == parentId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Get_Children_Fails()
    {
        // Arrange
        const long parentId = 10;

        var operation = new OperationResult<List<CategoryViewModel>>()
            .Failed(
                OperationErrorCode.NotFound,
                "Query Fails");

        _sender.Send(
                Arg.Any<GetCategoryChildrenByParentIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetChildrenByParentId(
            parentId,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(404);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<List<CategoryViewModel>>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryChildrenByParentIdQuery>(query =>
                query.ParentId == parentId),
            CancellationToken.None);
    }

    private static CategoryViewModel CreateCategory(
        long id,
        string title,
        long parentId)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = title,
            ParentId = parentId,
            IsRemoved = false,

            Actions = new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            }
        };
    }
}