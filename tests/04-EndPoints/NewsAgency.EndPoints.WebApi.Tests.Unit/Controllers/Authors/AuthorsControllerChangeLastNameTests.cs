using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Authors;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Authors;

public class AuthorsControllerChangeLastNameTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerChangeLastNameTests()
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
    public async Task Should_Return_Success_Result_And_Updated_Author_With_HATEOAS_Links()
    {
        // Arrange
        const long authorId = 10;

        var request = new ChangeAuthorLastNameRequest
        {
            LastName = "Updated LastName"
        };

        var author = CreateAuthor(authorId);

        var operation = new OperationResult<AuthorViewModel>()
            .Succeeded(author);

        _sender.Send(
                Arg.Any<ChangeAuthorLastNameCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.ChangeLastName(
            authorId,
            request,
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

        response.Data.Should().BeSameAs(author);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<ChangeAuthorLastNameCommand>(command =>
                command.Id == authorId &&
                command.LastName == request.LastName),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Change_Last_Name_Fails()
    {
        // Arrange
        const long authorId = 10;

        var request = new ChangeAuthorLastNameRequest
        {
            LastName = "Updated LastName"
        };

        var operation = new OperationResult<AuthorViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                "Query Fails");

        _sender.Send(
                Arg.Any<ChangeAuthorLastNameCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.ChangeLastName(
            authorId,
            request,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(409);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<AuthorViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        _urlHelper.DidNotReceive()
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<ChangeAuthorLastNameCommand>(command =>
                command.Id == authorId &&
                command.LastName == request.LastName),
            CancellationToken.None);
    }

    private static AuthorViewModel CreateAuthor(long id)
    {
        return new AuthorViewModel
        {
            Id = id,
            FirstName = "Mehdi",
            LastName = "Updated LastName",
            Email = $"author{id}@test.com",
            PhoneNumber = $"0912000000{id}"
        };
    }
}