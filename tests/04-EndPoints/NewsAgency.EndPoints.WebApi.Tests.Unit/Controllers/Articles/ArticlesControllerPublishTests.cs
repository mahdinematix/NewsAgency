using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Articles;

public class ArticlesControllerPublishTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly ArticlesController _controller;

    public ArticlesControllerPublishTests()
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
    public async Task Should_Return_Success_Result_And_Published_Article_With_HATEOAS_Links()
    {
        // Arrange
        const long articleId = 10;

        var article = CreateArticle(articleId);

        var operation = new OperationResult<ArticleViewModel>()
            .Succeeded(article);

        _sender.Send(
                Arg.Any<PublishArticleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Publish(
            articleId,
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

        await _sender.Received(1).Send(
            Arg.Is<PublishArticleCommand>(
                command => command.Id == articleId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Publish_Fails()
    {
        // Arrange
        const long articleId = 10;

        var operation = new OperationResult<ArticleViewModel>()
            .Failed(
                OperationErrorCode.LogicError,
                "Article cannot be published from its current status.");

        _sender.Send(
                Arg.Any<PublishArticleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Publish(
            articleId,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(422);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<ArticleViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();

        await _sender.Received(1).Send(
            Arg.Is<PublishArticleCommand>(
                command => command.Id == articleId),
            Arg.Any<CancellationToken>());
    }

    private static ArticleViewModel CreateArticle(long id)
    {
        return new ArticleViewModel
        {
            Id = id,
            Title = $"Article {id}",
            Content = $"Article {id} content",
            AuthorId = 1,
            CategoryId = 1,
            Status = "Published",

            Actions = new ArticleActionsViewModel
            {
                CanEdit = false,
                CanPublish = false,
                CanReject = false,
                CanArchive = true,
                CanMoveToDraft = false
            }
        };
    }
}