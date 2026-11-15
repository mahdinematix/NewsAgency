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

public class AuthorsControllerGetAllTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerGetAllTests()
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
    public async Task Should_Return_Success_Result_And_All_Authors_With_HATEOAS_Links()
    {
        // Arrange
        var authors = new List<AuthorViewModel>
        {
            CreateAuthor(1),
            CreateAuthor(2)
        };

        var operation = new OperationResult<List<AuthorViewModel>>()
            .Succeeded(authors);

        _sender.Send(
                Arg.Any<GetAllAuthorsQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        var response = AssertSuccessfulResponse(actionResult);

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().HaveCount(2);

        response.Data[0].Data.Should().BeSameAs(authors[0]);
        response.Data[1].Data.Should().BeSameAs(authors[1]);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Any<GetAllAuthorsQuery>(),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Empty_Collection_When_No_Authors_Exist()
    {
        // Arrange
        var operation = new OperationResult<List<AuthorViewModel>>()
            .Succeeded(new List<AuthorViewModel>());

        _sender.Send(
                Arg.Any<GetAllAuthorsQuery>(),
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
            Arg.Any<GetAllAuthorsQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_And_Error_Response_When_Query_Fails()
    {
        // Arrange
        var errorCode = OperationErrorCode.Validation;

        var operation = new OperationResult<List<AuthorViewModel>>()
            .Failed(
                errorCode,
                ApplicationMessages.ValueFailureToObserveMaxLength);

        _sender.Send(
                Arg.Any<GetAllAuthorsQuery>(),
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
            .BeOfType<ApiResponse<List<AuthorViewModel>>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        objectResult.StatusCode.Should().Be(400);

        response.ErrorCode.Should().Be(errorCode);

        response.Data.Should().BeNull();

        response.Links.Should().BeEmpty();

        _urlHelper.DidNotReceive()
            .Link(
                Arg.Any<string>(),
                Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Any<GetAllAuthorsQuery>(),
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

    private static ApiResponse<List<ApiResource<AuthorViewModel>>>
        AssertSuccessfulResponse(IActionResult actionResult)
    {
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        return okResult.Value.Should()
            .BeOfType<ApiResponse<List<ApiResource<AuthorViewModel>>>>()
            .Subject;
    }
}