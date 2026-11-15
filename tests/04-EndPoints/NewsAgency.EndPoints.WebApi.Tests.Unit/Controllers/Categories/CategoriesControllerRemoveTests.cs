using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Categories;

public class CategoriesControllerRemoveTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerRemoveTests()
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
    public async Task Should_Return_Success_Result_And_Removed_Category_With_HATEOAS_Links()
    {
        // Arrange
        const long categoryId = 10;

        var category = CreateCategory(categoryId);

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(category);

        _sender.Send(
                Arg.Any<RemoveCategoryCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.Remove(
            categoryId,
            CancellationToken.None);

        // Assert
        var okResult = actionResult.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value.Should()
            .BeOfType<ApiResponse<CategoryViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeTrue();

        response.Data.Should().Be(category);

        response.Data.IsRemoved.Should().BeTrue();

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<RemoveCategoryCommand>(
                command => command.Id == categoryId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Remove_Fails()
    {
        // Arrange
        const long categoryId = 10;

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                "remove fails.");

        _sender.Send(
                Arg.Any<RemoveCategoryCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.Remove(
            categoryId,
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
            .Link(Arg.Any<string>(), Arg.Any<object?>());

        await _sender.Received(1).Send(
            Arg.Is<RemoveCategoryCommand>(
                command => command.Id == categoryId),
            Arg.Any<CancellationToken>());
    }

    private static CategoryViewModel CreateCategory(long id)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = $"Category {id}",
            ParentId = null,
            IsRemoved = true,

            Actions = new CategoryActionsViewModel
            {
                CanRemove = false,
                CanRestore = true
            }
        };
    }
}