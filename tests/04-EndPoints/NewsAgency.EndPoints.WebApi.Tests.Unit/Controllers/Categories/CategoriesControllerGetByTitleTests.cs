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

public class CategoriesControllerGetByTitleTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerGetByTitleTests()
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
    public async Task Should_Return_Success_Result_And_Category_With_HATEOAS_Links()
    {
        // Arrange
        const string expectedTitle = "Programming";

        var category = CreateCategory(10, expectedTitle);

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(category);

        _sender.Send(
                Arg.Any<GetCategoryByTitleQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetByTitle(
            expectedTitle,
            CancellationToken.None);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<CategoryViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().Be(category);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryByTitleQuery>(
                query => query.Title == expectedTitle),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Query_Fails()
    {
        // Arrange
        const string expectedTitle = "Non Existing Category";

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                "query fails.");

        _sender.Send(
                Arg.Any<GetCategoryByTitleQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetByTitle(
            expectedTitle,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(404);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<CategoryViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryByTitleQuery>(
                query => query.Title == expectedTitle),
            Arg.Any<CancellationToken>());
    }

    private static CategoryViewModel CreateCategory(
        long id,
        string title)
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