using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Articles;

public class ArticlesControllerCreateTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly ArticlesController _controller;

    public ArticlesControllerCreateTests()
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
    public async Task Should_Return_Created_Response_With_Location_And_HATEOAS_Links()
    {
        // Arrange
        const long expectedArticleId = 10;

        var command = new CreateArticleCommand(
            "Test Article",
            "Test Article Content",
            1,
            1);

        var createdArticle = CreateArticle(expectedArticleId);

        var operation = new OperationResult<ArticleViewModel>()
            .Succeeded(createdArticle);

        _sender.Send(
                Arg.Any<CreateArticleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Create(
            command,
            CancellationToken.None);

        // Assert
        var createdResult = actionResult.Should()
            .BeOfType<CreatedAtRouteResult>()
            .Subject;

        createdResult.StatusCode.Should().Be(201);

        createdResult.RouteName.Should()
            .Be(ApiRouteNames.Articles.GetById);

        createdResult.RouteValues.Should().ContainKey("id");

        createdResult.RouteValues!["id"].Should()
            .Be(expectedArticleId);

        var response = createdResult.Value.Should()
            .BeOfType<ApiResponse<long>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().Be(expectedArticleId);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<CreateArticleCommand>(
                actual => ReferenceEquals(actual, command)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Creation_Fails()
    {
        // Arrange
        var command = new CreateArticleCommand(
            "Duplicate Article",
            "Duplicate Article Content",
            1,
            1);

        var operation = new OperationResult<ArticleViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                ApplicationMessages.ValueCannotBeDuplicated);

        _sender.Send(
                Arg.Any<CreateArticleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.Create(
            command,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(409);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<ArticleViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<CreateArticleCommand>(
                actual => ReferenceEquals(actual, command)),
            Arg.Any<CancellationToken>());
    }

    private static ArticleViewModel CreateArticle(long id)
    {
        return new ArticleViewModel
        {
            Id = id,
            Title = $"Article {id}",
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