using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Extensions;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Extensions;

public class CategoryResultExtensionsTests
{
    private readonly IUrlHelper _urlHelper;
    private readonly TestController _controller;

    public CategoryResultExtensionsTests()
    {
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper.Link(
                Arg.Any<string>(),
                Arg.Any<object?>())
            .Returns(callInfo =>
                $"/test/{callInfo.ArgAt<string>(0)}");

        _controller = new TestController
        {
            Url = _urlHelper
        };
    }

    private sealed class TestController : ControllerBase
    {
    }

    [Fact]
    public void Should_Return_Ok_With_Category_Links_When_Operation_Succeeds()
    {
        // Arrange
        var category = new CategoryViewModel
        {
            Id = 1,
            Title = "Test Category",
            ParentId = null,
            IsRemoved = false,
            Actions = new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            }
        };

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(category);

        // Act
        var actionResult = _controller.ToCategoryResult(operation);

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

        response.Links.Should().ContainKey("self");
        response.Links.Should().ContainKey("collection");

        response.Links["self"].Href.Should().NotBeNullOrEmpty();
        response.Links["self"].Method.Should().Be("GET");

        _urlHelper.Received().Link(
            Arg.Any<string>(),
            Arg.Any<object?>());
    }

    [Fact]
    public void Should_Return_Error_Response_Without_Generating_Links_When_Operation_Fails()
    {
        // Arrange
        const string expectedMessage = "Category not found.";

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                expectedMessage);

        // Act
        var actionResult = _controller.ToCategoryResult(operation);

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
        response.Message.Should().Be(expectedMessage);
        response.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        response.Links.Should().BeEmpty();

        _urlHelper.DidNotReceiveWithAnyArgs()
            .Link(default, default);
    }
}