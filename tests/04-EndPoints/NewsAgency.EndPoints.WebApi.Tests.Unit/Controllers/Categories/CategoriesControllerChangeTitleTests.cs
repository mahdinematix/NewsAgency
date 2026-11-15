using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Category.Common;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.EndPoints.WebApi.Controllers;
using NewsAgency.EndPoints.WebApi.Models;
using NewsAgency.EndPoints.WebApi.Models.Categories;
using NSubstitute;

namespace NewsAgency.EndPoints.WebApi.Tests.Unit.Controllers.Categories;

public class CategoriesControllerChangeTitleTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerChangeTitleTests()
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
    public async Task Should_Return_Success_Result_And_Updated_Category_With_HATEOAS_Links()
    {
        // Arrange
        const long categoryId = 10;

        var request = new ChangeCategoryTitleRequest
        {
            Title = "Updated Category"
        };

        var category = CreateCategory(categoryId);

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(category);

        _sender.Send(
                Arg.Any<ChangeCategoryTitleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.ChangeTitle(
            categoryId,
            request,
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

        response.Data.Should().BeSameAs(category);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<ChangeCategoryTitleCommand>(command =>
                command.Id == categoryId &&
                command.Title == request.Title),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Change_Title_Fails()
    {
        // Arrange
        const long categoryId = 10;

        var request = new ChangeCategoryTitleRequest
        {
            Title = "Updated Category"
        };

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                "Query Fails");

        _sender.Send(
                Arg.Any<ChangeCategoryTitleCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.ChangeTitle(
            categoryId,
            request,
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
            Arg.Is<ChangeCategoryTitleCommand>(command =>
                command.Id == categoryId &&
                command.Title == request.Title),
            CancellationToken.None);
    }

    private static CategoryViewModel CreateCategory(long id)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = "Updated Category",
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