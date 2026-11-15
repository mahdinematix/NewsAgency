using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Authors;

public class AuthorsControllerCreateTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerCreateTests()
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
    public async Task Should_Return_Created_Response_With_Location_And_HATEOAS_Links()
    {
        // Arrange
        const long expectedAuthorId = 10;

        var command = new CreateAuthorCommand(
            "Mehdi",
            "Nemati",
            "mehdi@test.com",
            "09121234567");

        var createdAuthor = CreateAuthor(expectedAuthorId);


        var operation = new OperationResult<AuthorViewModel>()
            .Succeeded(createdAuthor);

        _sender.Send(
                Arg.Any<CreateAuthorCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

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
            .Be(ApiRouteNames.Authors.GetById);

        createdResult.RouteValues.Should().ContainKey("id");

        createdResult.RouteValues!["id"].Should()
            .Be(expectedAuthorId);

        var response = createdResult.Value.Should()
            .BeOfType<ApiResponse<long>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().Be(expectedAuthorId);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<CreateAuthorCommand>(
                actual => ReferenceEquals(actual, command)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Creation_Fails()
    {
        // Arrange
        var command = new CreateAuthorCommand(
            "Mehdi",
            "Nemati",
            "duplicate@test.com",
            "09121234567");

        var operation = new OperationResult<AuthorViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                ApplicationMessages.ValueCannotBeDuplicated);

        _sender.Send(
                Arg.Any<CreateAuthorCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

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
            .BeOfType<ApiResponse<AuthorViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        _urlHelper.DidNotReceive()
            .Link(
                Arg.Any<string>(),
                Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<CreateAuthorCommand>(
                actual => ReferenceEquals(actual, command)),
            Arg.Any<CancellationToken>());
    }

    private static AuthorViewModel CreateAuthor(long id)
    {
        return new AuthorViewModel
        {
            Id = id,
            FirstName = "Mahdi",
            LastName = "Nemati",
            Email = "Mahdinematix@gmail.com",
            PhoneNumber = "009899978874"
            
        };
    }
}