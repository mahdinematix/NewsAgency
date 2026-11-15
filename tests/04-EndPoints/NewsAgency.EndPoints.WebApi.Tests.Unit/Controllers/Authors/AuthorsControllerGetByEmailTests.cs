using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Authors;

public class AuthorsControllerGetByEmailTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerGetByEmailTests()
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
    public async Task Should_Return_Success_Result_And_Author_With_Links_When_Author_Email_Exists()
    {
        // Arrange
        const string expectedEmail = "mehdi@test.com";

        var expectedAuthor = CreateAuthor(10, expectedEmail);

        var operation = new OperationResult<AuthorViewModel>()
            .Succeeded(expectedAuthor);

        _sender.Send(
                Arg.Any<GetAuthorByEmailQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetByEmail(
            expectedEmail,
            CancellationToken.None);

        // Assert
        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<AuthorViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();
        response.Data.Should().BeSameAs(expectedAuthor);
        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetAuthorByEmailQuery>(
                query => query.Email == expectedEmail),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_NotFound_Result_When_Author_Email_Does_Not_Exist()
    {
        // Arrange
        const string expectedEmail = "unknown@test.com";

        var operation = new OperationResult<AuthorViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                "query fails.");

        _sender.Send(
                Arg.Any<GetAuthorByEmailQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetByEmail(
            expectedEmail,
            CancellationToken.None);

        // Assert
        var notFoundResult = result.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        notFoundResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var response = notFoundResult.Value.Should()
            .BeOfType<ApiResponse<AuthorViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();
        response.Data.Should().BeNull();

        _urlHelper.DidNotReceive().Link(
            Arg.Any<string>(),
            Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<GetAuthorByEmailQuery>(
                query => query.Email == expectedEmail),
            Arg.Any<CancellationToken>());
    }

    private static AuthorViewModel CreateAuthor(long id, string email)
    {
        return new AuthorViewModel
        {
            Id = id,
            FirstName = "Mehdi",
            LastName = "Nemati",
            Email = email,
            PhoneNumber = $"0912000000{id}"
        };
    }
}