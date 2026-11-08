using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Articles;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Articles;

public class ArticlesControllerEditTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly ArticlesController _controller;

    public ArticlesControllerEditTests()
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
    public async Task Should_Return_Success_Result_And_Updated_Article_With_HATEOAS_Links()
    {
        // Arrange
        const long articleId = 10;

        var request = new EditArticleRequest
        {
            Title = "Updated Article",
            Content = "Updated Article Content",
            AuthorId = 2,
            CategoryId = 3
        };

        var article = CreateArticle(articleId);

        var operation = new OperationResult<ArticleViewModel>()
            .Succeeded(article);

        _sender.Send(
                Arg.Any<EditArticleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Edit(
            articleId,
            request,
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
            Arg.Is<EditArticleCommand>(command =>
                command.Id == articleId &&
                command.Title == request.Title &&
                command.Content == request.Content &&
                command.AuthorId == request.AuthorId &&
                command.CategoryId == request.CategoryId),
            CancellationToken.None);
    }


    [Fact]
    public async Task Should_Return_Failed_Result_When_Edit_Fails()
    {
        // Arrange
        const long articleId = 10;

        var request = new EditArticleRequest
        {
            Title = "Updated Article",
            Content = "Updated Article Content",
            AuthorId = 2,
            CategoryId = 3
        };

        var operation = new OperationResult<ArticleViewModel>()
            .Failed(
                OperationErrorCode.LogicError,
                "Operation cannot be performed in the current article status.");

        _sender.Send(
                Arg.Any<EditArticleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Edit(
            articleId,
            request,
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
            Arg.Is<EditArticleCommand>(command =>
                command.Id == articleId &&
                command.Title == request.Title &&
                command.Content == request.Content &&
                command.AuthorId == request.AuthorId &&
                command.CategoryId == request.CategoryId),
            Arg.Any<CancellationToken>());
    }

    private static ArticleViewModel CreateArticle(long id)
    {
        return new ArticleViewModel
        {
            Id = id,
            Title = "Updated Article",
            Content = "Updated Article Content",
            AuthorId = 2,
            CategoryId = 3,
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