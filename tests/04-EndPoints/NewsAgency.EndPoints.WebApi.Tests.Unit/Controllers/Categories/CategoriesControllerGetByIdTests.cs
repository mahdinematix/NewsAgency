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

public class CategoriesControllerGetByIdTests
{
    private readonly ISender _sender;
    private readonly IUrlHelper _urlHelper;
    private readonly CategoriesController _controller;

    public CategoriesControllerGetByIdTests()
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
    public async Task Should_Return_Success_Result_And_Category_By_Id_With_HATEOAS_Links()
    {
        // Arrange
        const long categoryId = 10;

        var category = CreateCategory(categoryId);

        var operation = new OperationResult<CategoryViewModel>()
            .Succeeded(category);

        _sender.Send(
                Arg.Any<GetCategoryByIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(operation);

        // Act
        var actionResult = await _controller.GetById(
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

        response.Links.Should().NotBeEmpty();

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryByIdQuery>(
                query => query.Id == categoryId),
            CancellationToken.None);
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Query_Fails()
    {
        // Arrange
        const long categoryId = 10;

        var operation = new OperationResult<CategoryViewModel>()
            .Failed(
                OperationErrorCode.NotFound,
                ApplicationMessages.RecordNotFound);

        _sender.Send(
                Arg.Any<GetCategoryByIdQuery>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(operation));

        // Act
        var actionResult = await _controller.GetById(
            categoryId,
            CancellationToken.None);

        // Assert
        var objectResult = actionResult.Should()
            .BeOfType<ObjectResult>()
            .Subject;

        objectResult.StatusCode.Should().Be(404);

        var response = objectResult.Value.Should()
            .BeOfType<ApiResponse<CategoryViewModel>>()
            .Subject;

        response.IsSuccess.Should().BeFalse();

        response.Data.Should().BeNull();

        await _sender.Received(1).Send(
            Arg.Is<GetCategoryByIdQuery>(
                query => query.Id == categoryId),
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