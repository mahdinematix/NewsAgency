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

public class CategoriesControllerChangeParentTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerChangeParentTests()
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
        const long parentId = 20;

        var request = new ChangeCategoryParentRequest
        {
            ParentId = parentId
        };

        var category = CreateCategory(categoryId, parentId);

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(category);

        _sender.Send(
                Arg.Any<ChangeCategoryParentCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.ChangeParent(
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

        response.Data.Should().Be(category);

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<ChangeCategoryParentCommand>(command =>
                command.Id == categoryId &&
                command.ParentId == request.ParentId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Change_Parent_Fails()
    {
        // Arrange
        const long categoryId = 10;
        const long parentId = 20;

        var request = new ChangeCategoryParentRequest
        {
            ParentId = parentId
        };

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.Conflict,
                "Category cannot be moved to the specified parent.");

        _sender.Send(
                Arg.Any<ChangeCategoryParentCommand>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.ChangeParent(
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
            Arg.Is<ChangeCategoryParentCommand>(command =>
                command.Id == categoryId &&
                command.ParentId == request.ParentId),
            CancellationToken.None);
    }

    private static CategoryViewModel CreateCategory(
        long id,
        long? parentId)
    {
        return new CategoryViewModel
        {
            Id = id,
            Title = "Updated Category",
            ParentId = parentId,
            IsRemoved = false,

            Actions = new CategoryActionsViewModel
            {
                CanRemove = true,
                CanRestore = false
            }
        };
    }
}