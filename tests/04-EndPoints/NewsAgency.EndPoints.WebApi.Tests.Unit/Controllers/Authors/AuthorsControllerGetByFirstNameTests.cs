using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Authors;

public class AuthorsControllerGetByFirstNameTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerGetByFirstNameTests()
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
    public async Task Should_Return_Success_Result_And_Author_With_HATEOAS_Links()
    {
        // Arrange
        const string expectedFirstName = "Mehdi";

        var author = CreateAuthor(10, expectedFirstName);

        var operation = new OperationResult<AuthorViewModel>()
            .Succeeded(author);

        _sender.Send(
                Arg.Any<GetAuthorByFirstNameQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetByFirstName(
            expectedFirstName,
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
            Arg.Is<GetAuthorByFirstNameQuery>(
                query => query.FirstName == expectedFirstName),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Query_Fails()
    {
        // Arrange
        const string expectedFirstName = "Non Existing Author";

        var operation = new OperationResult<AuthorViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                "query fails.");

        _sender.Send(
                Arg.Any<GetAuthorByFirstNameQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetByFirstName(
            expectedFirstName,
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

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<GetAuthorByFirstNameQuery>(
                query => query.FirstName == expectedFirstName),
            Arg.Any<CancellationToken>());
    }

    private static AuthorViewModel CreateAuthor(
        long id,
        string firstName)
    {
        return new AuthorViewModel
        {
            Id = id,
            FirstName = firstName,
            LastName = "Test LastName",
            Email = $"author{id}@test.com",
            PhoneNumber = $"0912000000{id}"
        };
    }
}