using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Articles;

public class ArticlesControllerGetByTitleTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly ArticlesController _controller;

    public ArticlesControllerGetByTitleTests()
    {
        _sender = Substitute.For<ISender>();
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper.Link(
                Arg.Any<string>(),
                Arg.Any<object?>())
            .Returns("/test");

        _controller = new ArticlesController(_sender)
        {
            Url = _urlHelper
        };
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Article_With_HATEOAS_Links()
    {
        // Arrange
        const string expectedTitle = "Learning ASP.NET Core";

        var article = CreateArticle(10, expectedTitle);

        var operation = new OperationResult<ArticleViewModel>()
            .Succeeded(article);

        _sender.Send(
                Arg.Any<GetArticleByTitleQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

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
            .BeOfType<ApiResponse<ArticleViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeSameAs(article);

        response.Links.Should().NotBeEmpty();

        // Verify MediatR interaction
        await _sender.Received(1).Send(
            Arg.Is<GetArticleByTitleQuery>(
                query => query.Title == expectedTitle),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Query_Fails()
    {
        // Arrange
        const string expectedTitle = "Non Existing Article";

        const string expectedMessage = "Article not found.";

        var operation = new OperationResult<ArticleViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                expectedMessage);

        _sender.Send(
                Arg.Any<GetArticleByTitleQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

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
            .BeOfType<ApiResponse<ArticleViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();

        await _sender.Received(1).Send(
            Arg.Is<GetArticleByTitleQuery>(
                query => query.Title == expectedTitle),
            Arg.Any<CancellationToken>());
    }

    private static ArticleViewModel CreateArticle(
        long id,
        string title)
    {
        return new ArticleViewModel
        {
            Id = id,
            Title = title,
            Content = $"Content of article {id}",
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
    }
}