using FluentAssertions;
using NewsAgency.Application.CategoryServices.CommandHandlers.Remove;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Linq.Expressions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.Remove;

public class RemoveCategoryCommandHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly RemoveCategoryCommandHandler _commandHandler;
    private readonly CategoryTestBuilder _builder;

    public RemoveCategoryCommandHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _commandRepository = Substitute.For<ICategoryCommandRepository>();
        _commandHandler = new RemoveCategoryCommandHandler(_commandRepository, _queryRepository);
        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Remove_And_Return_Success_Result_When_Command_Is_Valid()
    {
        //arrange
        var category = _builder.Build();
        var command = new RemoveCategoryCommand(category.Id);
        _queryRepository.GetByAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        category.IsRemoved.Should().BeTrue();
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

    [Fact]
    public async Task Should_Return_Failed_Result_When_Record_Not_Found()
    {
        // Arrange
        const long categoryId = 100;
        var command = new RemoveCategoryCommand(
            categoryId);

        _queryRepository
            .GetByAsync(categoryId, Arg.Any<CancellationToken>())
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
    public async Task Should_Return_Failed_Result_When_Category_Has_Active_Children()
    {
        // Arrange
        var category = _builder.Build();
        var command = new RemoveCategoryCommand(category.Id);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Conflict);
        category.IsRemoved.Should().BeFalse();

        await _queryRepository.Received(1)
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>());

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_Without_Saving_When_Category_Is_Already_Removed()
    {
        // Arrange
        var category = _builder.Build();
        category.Remove(false);

        var command = new RemoveCategoryCommand(category.Id);

        _queryRepository
            .GetByAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.IsRemoved.Should().BeTrue();

        await _queryRepository.Received(1)
            .GetByAsync(
                category.Id,
                Arg.Any<CancellationToken>());

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Parent_Category_Does_Not_Exist()
    {
        // Arrange
        const long categoryId = 100;
        const long parentId = 200;

        var category = _builder.WithParentId(parentId).Build();

        var command = new RemoveCategoryCommand(
            categoryId);

        _queryRepository
            .GetByAsync(
                categoryId,
                Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .GetByAsync(
                parentId,
                Arg.Any<CancellationToken>())
            .ReturnsNull();


        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);

        result.Data.Should().BeNull();

        await _queryRepository.Received(2).GetByAsync(
            Arg.Any<long>(),
            CancellationToken.None);

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(
                Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task Should_Use_Parent_Removed_State_When_Returning_Actions()
    {
        // Arrange
        const long categoryId = 100;
        const long parentId = 200;

        var category = _builder
            .WithParentId(parentId)
            .Build();

        var parent = new Category(
            new CategoryTitle("Parent Category"));

        parent.Remove(hasActiveChildren: false);

        var command = new RemoveCategoryCommand(categoryId);

        _queryRepository
            .GetByAsync(
                categoryId,
                Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .GetByAsync(
                parentId,
                Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Data.Should().NotBeNull();

        result.Data!.IsRemoved.Should().BeTrue();

        result.Data.Actions.CanRestore.Should().BeFalse();

        await _queryRepository.Received(1).GetByAsync(
            categoryId,
            CancellationToken.None);

        await _queryRepository.Received(1).GetByAsync(
            parentId,
            CancellationToken.None);

        await _commandRepository.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }
}