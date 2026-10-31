using System.Linq.Expressions;
using FluentAssertions;
using NewsAgency.Application.CategoryServices.CommandHandlers.ChangeTitle;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.ChangeTitle;

public class ChangeCategoryTitleCommandHandlerTests
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly ChangeCategoryTitleCommandHandler _commandHandler;
    private readonly CategoryTestBuilder _builder;

    public ChangeCategoryTitleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<ICategoryCommandRepository>();
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _commandHandler = new ChangeCategoryTitleCommandHandler(_commandRepository, _queryRepository);
        _builder = new CategoryTestBuilder();
    }
    [Fact]
    public async Task Should_Change_Category_Title()
    {
        //arrange
        var category = _builder.Build();
        string newTitle = "newTitle";
        var command = new ChangeCategoryTitleCommand(category.Id, newTitle);
        _queryRepository.GetByAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        category.Title.Value.Should().Be(newTitle);
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        //act
        var result = await _commandHandler.Handle(null!, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryRepository.DidNotReceive().GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_Title_Is_Invalid(int length)
    {
        //arrange
        var category = _builder.Build();
        var invalidTitle = "A".PadLeft(length, '-');
        var command = new ChangeCategoryTitleCommand(category.Id, invalidTitle);
        _queryRepository.GetByAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Record_Not_Found()
    {
        // Arrange
        const long CategoryId = 100;
        var command = new ChangeCategoryTitleCommand(
            CategoryId,
            "newTitle@example.com");

        _queryRepository
            .GetByAsync(CategoryId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Title_Of_Category_To_Change_Exist()
    {
        //arrange
        const long categoryId = 200;
        string sameTitle = "SameTitle@test.test";
        var category = _builder.WithTitle(new CategoryTitle(sameTitle)).Build();
        var command = new ChangeCategoryTitleCommand(
            categoryId, sameTitle);
        _queryRepository
            .GetByAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(category);
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Category, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);


        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Conflict);
        await _queryRepository.Received(1)
            .Exists(
                Arg.Any<Expression<Func<Category, bool>>>(),
                Arg.Any<CancellationToken>());
        await _commandRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}