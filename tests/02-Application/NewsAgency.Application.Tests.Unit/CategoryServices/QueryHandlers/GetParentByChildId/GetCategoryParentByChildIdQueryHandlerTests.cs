using FluentAssertions;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetParentByChildId;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetParentByChildId;

public class GetCategoryParentByChildIdQueryHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly GetCategoryParentByChildIdQueryHandler _queryHandler;
    private readonly CategoryTestBuilder _builder;

    public GetCategoryParentByChildIdQueryHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();

        _queryHandler = new GetCategoryParentByChildIdQueryHandler(
            _queryRepository);

        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Parent_Category_When_Parent_Exists()
    {
        // Arrange
        const long childId = 100;

        var child = _builder.Build();
        var parent = _builder.Build();

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(child);

        _queryRepository
            .GetParentByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(parent.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryParentByChildIdQuery(childId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Id.Should().Be(parent.Id);
        result.Data.Title.Should().Be(parent.Title.Value);
        result.Data.IsRemoved.Should().Be(parent.IsRemoved);
        result.Data.ParentId.Should().Be(parent.ParentId);

        result.Data.Actions.CanRemove.Should().BeTrue();
        result.Data.Actions.CanRestore.Should().BeFalse();

        await _queryRepository.Received(1).GetByAsync(
            childId,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(1).GetParentByAsync(
            childId,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(1).HasActiveChildrenAsync(
            parent.Id,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_With_Null_Data_When_Category_Has_No_Parent()
    {
        // Arrange
        const long childId = 100;

        var child = _builder.Build();

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(child);

        _queryRepository
            .GetParentByAsync(childId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryParentByChildIdQuery(childId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            childId,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(1).GetParentByAsync(
            childId,
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_NotFound_When_Child_Category_Does_Not_Exist()
    {
        // Arrange
        const long childId = 100;

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryParentByChildIdQuery(childId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            childId,
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().GetParentByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Validation_When_Request_Is_Null()
    {
        // Act
        var result = await _queryHandler.Handle(
            null!,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        result.Data.Should().BeNull();

        await _queryRepository.DidNotReceive().GetByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().GetParentByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Grandparent_Does_Not_Exist()
    {
        // Arrange
        const long childId = 100;
        const long grandparentId = 500;

        var child = _builder.Build();
        var parent = new Category(_builder.Build().Title, grandparentId);

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(child);

        _queryRepository
            .GetParentByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .GetByAsync(grandparentId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryParentByChildIdQuery(childId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            grandparentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Should_Set_CanRestore_Based_On_Grandparent_Removed_State(
        bool grandparentIsRemoved,
        bool expectedCanRestore)
    {
        // Arrange
        const long childId = 100;
        const long grandparentId = 500;

        var child = _builder.Build();

        var parent = new Category(
            _builder.Build().Title,
            grandparentId);

        parent.Remove(hasActiveChildren: false);

        var grandparent = _builder.Build();

        if (grandparentIsRemoved)
        {
            grandparent.Remove(hasActiveChildren: false);
        }

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(child);

        _queryRepository
            .GetParentByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .GetByAsync(grandparentId, Arg.Any<CancellationToken>())
            .Returns(grandparent);

        _queryRepository
            .HasActiveChildrenAsync(parent.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryParentByChildIdQuery(childId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.IsRemoved.Should().BeTrue();

        result.Data.Actions.CanRestore.Should()
            .Be(expectedCanRestore);

        await _queryRepository.Received(1).GetByAsync(
            grandparentId,
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Should_Set_CanRemove_Based_On_Active_Children(
        bool hasActiveChildren,
        bool expectedCanRemove)
    {
        // Arrange
        const long childId = 100;

        var child = _builder.Build();
        var parent = _builder.Build();

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(child);

        _queryRepository
            .GetParentByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(parent.Id, Arg.Any<CancellationToken>())
            .Returns(hasActiveChildren);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryParentByChildIdQuery(childId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        result.Data.Actions.CanRemove.Should()
            .Be(expectedCanRemove);

        await _queryRepository.Received(1).HasActiveChildrenAsync(
            parent.Id,
            Arg.Any<CancellationToken>());
    }
}