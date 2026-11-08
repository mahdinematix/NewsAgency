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

public class ArticlesControllerArchiveTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly ArticlesController _controller;

    public ArticlesControllerArchiveTests()
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
    public async Task Should_Return_Success_Result_And_Archived_Article_With_HATEOAS_Links()
    {
        // Arrange
        const long articleId = 10;

        var article = CreateArticle(articleId);

        var operation = new OperationResult<ArticleViewModel>()
            .Succeeded(article);

        _sender.Send(
            Arg.Any<ArchiveArticleCommand>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Archive(
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
        response.Message.Should().Be(operation.Message);
        response.ErrorCode.Should().Be(operation.ErrorCode);

        response.Links.Should().NotBeEmpty();

        // Verify MediatR interaction
        await _sender.Received(1).Send(
            Arg.Is<ArchiveArticleCommand>(
                command => command.Id == articleId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Archive_Fails()
    {
        // Arrange
        const long articleId = 10;

        var operation = new OperationResult<ArticleViewModel>()
            .Failed(
                OperationErrorCode.LogicError,
                "Article cannot be archived from its current status.");

        _sender.Send(
            Arg.Any<ArchiveArticleCommand>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Archive(
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

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<ArchiveArticleCommand>(
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
            Status = "Archived",

            Actions = new ArticleActionsViewModel
            {
                CanEdit = false,
                CanPublish = false,
                CanReject = false,
                CanArchive = false,
                CanMoveToDraft = true
            }
        };
    }
}
