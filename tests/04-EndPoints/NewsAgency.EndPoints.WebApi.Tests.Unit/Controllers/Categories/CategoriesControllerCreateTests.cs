using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Routing;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Categories;

public class CategoriesControllerCreateTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerCreateTests()
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
    public async Task Should_Return_Created_Response_With_Location_And_HATEOAS_Links()
    {
        // Arrange
        const long expectedCategoryId = 10;

        var command = new CreateCategoryCommand(
            "Test Category",
            null);

        var createdCategory = CreateCategory(expectedCategoryId);

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(createdCategory);

        _sender.Send(
                Arg.Any<CreateCategoryCommand>(),
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
            .Be(ApiRouteNames.Categories.GetById);

        createdResult.RouteValues.Should().ContainKey("id");

        createdResult.RouteValues!["id"].Should()
            .Be(expectedCategoryId);

        var response = createdResult.Value.Should()
            .BeOfType<ApiResponse<long>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().Be(expectedCategoryId);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<CreateCategoryCommand>(
                actual => ReferenceEquals(actual, command)),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Creation_Fails()
    {
        // Arrange
        var command = new CreateCategoryCommand(
            "Duplicate Category",
            null);

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                ApplicationMessages.ValueCannotBeDuplicated);

        _sender.Send(
                Arg.Any<CreateCategoryCommand>(),
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
            .BeOfType<ApiResponse<CategoryViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        _urlHelper.DidNotReceive()
            .Link(
                Arg.Any<string>(),
                Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<CreateCategoryCommand>(
                actual => ReferenceEquals(actual, command)),
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
}