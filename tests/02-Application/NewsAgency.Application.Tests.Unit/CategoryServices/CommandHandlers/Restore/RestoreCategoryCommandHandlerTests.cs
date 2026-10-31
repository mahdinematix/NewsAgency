using FluentAssertions;
using NewsAgency.Application.CategoryServices.CommandHandlers.Restore;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.Restore;

public class RestoreCategoryCommandHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly RestoreCategoryCommandHandler _commandHandler;
    private readonly CategoryTestBuilder _builder;

    public RestoreCategoryCommandHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _commandRepository = Substitute.For<ICategoryCommandRepository>();

        _commandHandler = new RestoreCategoryCommandHandler(
            _commandRepository,
            _queryRepository);

        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Restore_And_Return_Success_Result_When_Parent_Is_Not_Removed()
    {
        // Arrange
        var category = _builder.Build();
        category.Remove(false);

        var command = new RestoreCategoryCommand(category.Id);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .IsParentRemovedAsync(
                category.ParentId,
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.IsRemoved.Should().BeFalse();

        await _queryRepository.Received(1)
            .IsParentRemovedAsync(
                category.ParentId,
                Arg.Any<CancellationToken>());

        await _commandRepository.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Validation_Result_When_Request_Is_Null()
    {
        // Act
        var result = await _commandHandler.Handle(
            null!,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Record_Not_Found()
    {
        // Arrange
        const long categoryId = 100;

        var command = new RestoreCategoryCommand(categoryId);

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
    public async Task Should_Return_Failed_Result_When_Parent_Is_Removed()
    {
        // Arrange
        var child = _builder.Build();

        child.ChangeParent(500);
        child.Remove(false);

        var command = new RestoreCategoryCommand(child.Id);

        _queryRepository.GetByAsync(child.Id, Arg.Any<CancellationToken>()).Returns(child);

        _queryRepository.IsParentRemovedAsync(500, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Conflict);
        child.IsRemoved.Should().BeTrue();

        await _queryRepository.Received(1)
            .IsParentRemovedAsync(
                500,
                Arg.Any<CancellationToken>());

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_Without_Saving_When_Category_Is_Already_Active()
    {
        // Arrange
        var category = _builder.Build();
        var command = new RestoreCategoryCommand(category.Id);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        category.IsRemoved.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}