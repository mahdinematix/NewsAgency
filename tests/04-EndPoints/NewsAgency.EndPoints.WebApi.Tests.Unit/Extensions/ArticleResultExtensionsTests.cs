using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Extensions;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Extensions;

public class ArticleResultExtensionsTests
{
    private readonly IUrlHelper _urlHelper;
    private readonly TestController _controller;

    public ArticleResultExtensionsTests()
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
    public void Should_Return_Ok_With_Article_Links_When_Operation_Succeeds()
    {
        // Arrange
        var article = new ArticleViewModel
        {
            Id = 1,
            Title = "Test Article",
            Content = "Test Article Content",
            AuthorId = 1,
            CategoryId = 1,
            Status = "Draft",
            Actions = new ArticleActionsViewModel
            {
                CanEdit = true,
                CanPublish = true,
                CanReject = true,
                CanArchive = false,
                CanMoveToDraft = false
            }
        };

        var operation = new OperationResult<ArticleViewModel>()
            .Succeeded(article);

        // Act
        var actionResult = _controller.ToArticleResult(operation);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<ArticleViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeSameAs(article);

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
        const string expectedMessage = "Article not found.";

        var operation = new OperationResult<ArticleViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                expectedMessage);

        // Act
        var actionResult = _controller.ToArticleResult(operation);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(404);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<ArticleViewModel>>()
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