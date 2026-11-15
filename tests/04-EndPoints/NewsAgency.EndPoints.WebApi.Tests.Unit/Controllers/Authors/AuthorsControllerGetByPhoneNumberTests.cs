using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Author.Common;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Authors;

public class AuthorsControllerGetByPhoneNumberTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly AuthorsController _controller;

    public AuthorsControllerGetByPhoneNumberTests()
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
    public async Task Should_Return_Success_Result_And_Author_With_Links_When_Author_Phone_Number_Exists()
    {
        // Arrange
        const string expectedPhoneNumber = "09121234567";

        var expectedAuthor = CreateAuthor(10, expectedPhoneNumber);

        var operation = new OperationResult<AuthorViewModel>().Succeeded(expectedAuthor);
        _sender.Send(
                Arg.Any<GetAuthorByPhoneNumberQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetByPhoneNumber(
            expectedPhoneNumber,
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
            Arg.Is<GetAuthorByPhoneNumberQuery>(
                query => query.PhoneNumber == expectedPhoneNumber),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_NotFound_Result_When_Author_Phone_Number_Does_Not_Exist()
    {
        // Arrange
        const string expectedPhoneNumber = "09999999999";

        var operation = new OperationResult<AuthorViewModel>().Failed(OperationErrorCode.NotFound, "query fails");


        _sender.Send(
                Arg.Any<GetAuthorByPhoneNumberQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var result = await _controller.GetByPhoneNumber(
            expectedPhoneNumber,
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
            Arg.Is<GetAuthorByPhoneNumberQuery>(
                query => query.PhoneNumber == expectedPhoneNumber),
            Arg.Any<CancellationToken>());
    }

    private static AuthorViewModel CreateAuthor(long id, string phoneNumber)
    {
        return new AuthorViewModel
        {
            Id = id,
            FirstName = "Mehdi",
            LastName = "Nemati",
            Email = $"author{id}@test.com",
            PhoneNumber = phoneNumber
        };
    }
}