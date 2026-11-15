using FluentAssertions;
using NewsAgency.Application.CategoryServices.CommandHandlers.ChangeParent;
using NewsAgency.Application.Contracts.Category.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.CategoryAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.CommandHandlers.ChangeParent;

public class ChangeCategoryParentCommandHandlerTests
{
    private readonly ICategoryCommandRepository _commandRepository;
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly ChangeCategoryParentCommandHandler _commandHandler;
    private readonly CategoryTestBuilder _builder;

    public ChangeCategoryParentCommandHandlerTests()
    {
        _commandRepository = Substitute.For<ICategoryCommandRepository>();
        _queryRepository = Substitute.For<ICategoryQueryRepository>();

        _commandHandler = new ChangeCategoryParentCommandHandler(
            _commandRepository,
            _queryRepository);

        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Change_Category_Parent()
    {
        // Arrange
        var category = _builder.Build();
        var parent = _builder.WithTitle(new CategoryTitle("Parent")).Build();

        var command = new ChangeCategoryParentCommand(
            category.Id,
            parent.Id);

        const bool hasActiveChildren = false;

        _queryRepository
            .GetByAsync(category.Id, CancellationToken.None)
            .Returns(category);

        _queryRepository
            .HasActiveChildrenAsync(category.Id, CancellationToken.None)
            .Returns(Task.FromResult(hasActiveChildren));

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(category.Id);
        result.Data.Title.Should().Be(category.Title.Value);
        result.Data.IsRemoved.Should().Be(category.IsRemoved);
        result.Data.ParentId.Should().Be(parent.Id);
        result.Data.Actions.Should().NotBeNull();

        result.Data.Actions.CanRemove
            .Should().Be(category.CanRemove(hasActiveChildren));

        result.Data.Actions.CanRestore
            .Should().Be(category.CanRestore(parent.IsRemoved));

        category.ParentId.Should().Be(parent.Id);

        await _queryRepository.Received(2)
            .GetByAsync(category.Id, CancellationToken.None);

        await _queryRepository.Received(1)
            .HasActiveChildrenAsync(category.Id, CancellationToken.None);

        await _commandRepository.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }

    
    [Fact]
    public async Task Should_Return_Success_And_Change_Category_Parent_To_Null()
    {
        // Arrange
        var category = _builder.Build();
        category.ChangeParent(100);

        var command = new ChangeCategoryParentCommand(
            category.Id,
            null);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Data.ParentId.Should().BeNull();

        result.Data.Actions.CanRemove
            .Should().Be(category.CanRemove(false));

        result.Data.Actions.CanRestore
            .Should().Be(category.CanRestore(false));

        await _queryRepository.Received(1)
            .GetByAsync(category.Id, Arg.Any<CancellationToken>());

        await _queryRepository.Received(1)
            .HasActiveChildrenAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>());

        await _commandRepository.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        // Act
        var result = await _commandHandler.Handle(
            null!,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);

        await _queryRepository.DidNotReceive()
            .GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive()
            .HasActiveChildrenAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>());

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Category_Not_Found()
    {
        // Arrange
        const long categoryId = 100;

        var command = new ChangeCategoryParentCommand(
            categoryId,
            200);

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

        await _queryRepository.Received(1)
            .GetByAsync(categoryId, Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive()
            .HasActiveChildrenAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>());

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_New_Parent_Not_Found()
    {
        // Arrange
        var category = _builder.Build();
        const long parentId = 999999;

        var originalParentId = category.ParentId;

        var command = new ChangeCategoryParentCommand(
            category.Id,
            parentId);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);

        category.ParentId.Should().Be(originalParentId);

        await _queryRepository.Received(1)
            .GetByAsync(category.Id, Arg.Any<CancellationToken>());

        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}