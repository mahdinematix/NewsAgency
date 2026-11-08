using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Article.Common;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Articles;

public class ArticlesControllerGetAllTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly ArticlesController _controller;

    public ArticlesControllerGetAllTests()
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
    public async Task Should_Return_Success_Result__And_All_Articles_With_HATEOAS_Links()
    {
        // Arrange
        var articles = new List<ArticleViewModel>
        {
            CreateArticle(1),
            CreateArticle(2)
        };

        var operation = new OperationResult<List<ArticleViewModel>>()
            .Succeeded(articles);

        _sender.Send(
            Arg.Any<GetAllArticlesQuery>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.GetAll(CancellationToken.None);

        // Assert
        var response = AssertSuccessfulResponse(actionResult);

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().HaveCount(2);

        response.Data[0].Data.Should().BeSameAs(articles[0]);
        response.Data[1].Data.Should().BeSameAs(articles[1]);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Any<GetAllArticlesQuery>(),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Empty_Collection_When_No_Articles_Exist()
    {
        // Arrange
        var operation = new OperationResult<List<ArticleViewModel>>()
            .Succeeded(new List<ArticleViewModel>());

        _sender.Send(
            Arg.Any<GetAllArticlesQuery>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        
        var response = AssertSuccessfulResponse(actionResult);
        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeEmpty();
        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Any<GetAllArticlesQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_And_Error_Response_When_Query_Fails()
    {
        // Arrange
        var errorCode = OperationErrorCode.Validation;
        var operation = new OperationResult<List<ArticleViewModel>>()
            .Failed(
               errorCode,
                ApplicationMessages.ValueFailureToObserveMaxLength);

        _sender.Send(
            Arg.Any<GetAllArticlesQuery>(),
            Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<List<ArticleViewModel>>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        objectResult.StatusCode.Should().Be(400);

        objectResult.Value.Should()
            .BeOfType<ApiResponse<List<ArticleViewModel>>>();

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Any<GetAllArticlesQuery>(),
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

    private static ApiResponse<List<ApiResource<ArticleViewModel>>>
        AssertSuccessfulResponse(IActionResult actionResult)
    {
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        return okResult.Value.Should()
            .BeOfType<ApiResponse<List<ApiResource<ArticleViewModel>>>>()
            .Subject;
    }
}