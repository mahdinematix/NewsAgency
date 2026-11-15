using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Categories;

public class CategoriesControllerGetAllTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerGetAllTests()
    {
        _sender = Substitute.For<ISender>();
        _urlHelper = Substitute.For<IUrlHelper>();

        _urlHelper.Link(
                Arg.Any<string>(),
                Arg.Any<object?>())
            .Returns("/test");

        _controller = new CategoriesController(_sender)
        {
            Url = _urlHelper
        };
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_All_Categories_With_HATEOAS_Links()
    {
        // Arrange
        var categories = new List<CategoryViewModel>
        {
            CreateCategory(1),
            CreateCategory(2)
        };

        var operation = new OperationResult<List<CategoryViewModel>>()
            .Succeeded(categories);

        _sender.Send(
                Arg.Any<GetAllCategoriesQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetAll(
            CancellationToken.None);

        // Assert
        var response = AssertSuccessfulResponse(actionResult);

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().HaveCount(2);

        response.Data[0].Data.Should().BeSameAs(categories[0]);
        response.Data[1].Data.Should().BeSameAs(categories[1]);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Any<GetAllCategoriesQuery>(),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Empty_Collection_When_No_Categories_Exist()
    {
        // Arrange
        var operation = new OperationResult<List<CategoryViewModel>>()
            .Succeeded(new List<CategoryViewModel>());

        _sender.Send(
                Arg.Any<GetAllCategoriesQuery>(),
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
            Arg.Any<GetAllCategoriesQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_And_Error_Response_When_Query_Fails()
    {
        // Arrange
        var errorCode = OperationErrorCode.Validation;

        var operation = new OperationResult<List<CategoryViewModel>>()
            .Failed(
                errorCode,
                ApplicationMessages.ValueFailureToObserveMaxLength);

        _sender.Send(
                Arg.Any<GetAllCategoriesQuery>(),
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
            .BeOfType<ApiResponse<List<CategoryViewModel>>>()
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
            Arg.Any<GetAllCategoriesQuery>(),
            Arg.Any<CancellationToken>());
    }

    private static CategoryViewModel CreateCategory(long id)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = $"Category {id}",
            ParentId = null,
            IsRemoved = false,
            Actions = new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            }
        };
    }

    private static ApiResponse<List<ApiResource<CategoryViewModel>>>
        AssertSuccessfulResponse(IActionResult actionResult)
    {
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        return okResult.Value.Should()
            .BeOfType<ApiResponse<List<ApiResource<CategoryViewModel>>>>()
            .Subject;
    }
}