using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Authors;

public class AuthorsControllerGetByIdTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerGetByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper.Link(
                Arg.Any<string>(),
                Arg.Any<object?>())
            .Returns("/test");

        _controller = new AuthorsController(_sender)
        {
            Url = _urlHelper
        };
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Author_By_Id_With_HATEOAS_Links()
    {
        // Arrange
        const long authorId = 10;

        var author = CreateAuthor(authorId);

        var operation = new OperationResult<AuthorViewModel>()
            .Succeeded(author);

        _sender.Send(
                Arg.Any<GetAuthorByIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetById(
            authorId,
            CancellationToken.None);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<AuthorViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().Be(author);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetAuthorByIdQuery>(
                query => query.Id == authorId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Query_Fails()
    {
        // Arrange
        const long authorId = 10;

        var operation = new OperationResult<AuthorViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                ApplicationMessages.RecordNotFound);

        _sender.Send(
                Arg.Any<GetAuthorByIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.GetById(
            authorId,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(404);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<AuthorViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        await _sender.Received(1).Send(
            Arg.Is<GetAuthorByIdQuery>(
                query => query.Id == authorId),
            Arg.Any<CancellationToken>());
    }

    private static AuthorViewModel CreateAuthor(long id)
    {
        return new AuthorViewModel
        {
            Id = id,
            FirstName = $"FirstName {id}",
            LastName = $"LastName {id}",
            Email = $"author{id}@test.com",
            PhoneNumber = $"0912000000{id}"
        };
    }
}