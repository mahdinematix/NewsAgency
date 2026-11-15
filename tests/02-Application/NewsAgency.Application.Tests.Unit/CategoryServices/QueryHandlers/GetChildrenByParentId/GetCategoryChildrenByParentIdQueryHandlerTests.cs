using FluentAssertions;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetChildrenByParentId;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetChildrenByParentId;

public class GetCategoryChildrenByParentIdQueryHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly GetCategoryChildrenByParentIdQueryHandler _queryHandler;
    private readonly CategoryTestBuilder _builder;

    public GetCategoryChildrenByParentIdQueryHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _queryHandler = new GetCategoryChildrenByParentIdQueryHandler(_queryRepository);
        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_List_Of_Category_Children_By_ParentId()
    {
        // Arrange
        const long parentId = 100;

        var parent = _builder.Build();

        var child1 = new Category(_builder.Build().Title, parentId);
        var child2 = new Category(_builder.Build().Title, parentId);

        var children = new List<Category>
        {
            child1,
            child2
        };

        _queryRepository
            .GetChildrenByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(children);

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryChildrenByParentIdQuery(parentId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);

        result.Data[0].Id.Should().Be(child1.Id);
        result.Data[0].Title.Should().Be(child1.Title.Value);
        result.Data[0].ParentId.Should().Be(parentId);
        result.Data[0].IsRemoved.Should().BeFalse();
        result.Data[0].Actions.CanRemove.Should().BeTrue();
        result.Data[0].Actions.CanRestore.Should().BeFalse();

        result.Data[1].Id.Should().Be(child2.Id);
        result.Data[1].Title.Should().Be(child2.Title.Value);
        result.Data[1].ParentId.Should().Be(parentId);
        result.Data[1].IsRemoved.Should().BeFalse();
        result.Data[1].Actions.CanRemove.Should().BeTrue();
        result.Data[1].Actions.CanRestore.Should().BeFalse();

        await _queryRepository.Received(1).GetChildrenByAsync(
            parentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(2).GetByAsync(
            parentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(2).HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        // Act
        var result = await _queryHandler.Handle(
            null!,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        result.Data.Should().BeNull();

        await _queryRepository.DidNotReceive().GetChildrenByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        // Arrange
        const long parentId = 100;

        _queryRepository
            .GetChildrenByAsync(parentId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryChildrenByParentIdQuery(parentId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetChildrenByAsync(
            parentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().GetByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_Result_With_Empty_List_When_Parent_Has_No_Children()
    {
        // Arrange
        const long parentId = 100;

        _queryRepository
            .GetChildrenByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(new List<Category>());

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryChildrenByParentIdQuery(parentId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();

        await _queryRepository.Received(1).GetChildrenByAsync(
            parentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().GetByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Parent_Does_Not_Exist()
    {
        // Arrange
        const long parentId = 100;

        var child = new Category(_builder.Build().Title, parentId);

        _queryRepository
            .GetChildrenByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(new List<Category> { child });

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryChildrenByParentIdQuery(parentId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            parentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().HasActiveChildrenAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public async Task Should_Set_CanRestore_Based_On_Parent_Removed_State(
        bool parentIsRemoved,
        bool expectedCanRestore)
    {
        // Arrange
        const long parentId = 100;

        var child = new Category(_builder.Build().Title, parentId);
        child.Remove(hasActiveChildren: false);

        var parent = _builder.Build();

        if (parentIsRemoved)
        {
            parent.Remove(hasActiveChildren: false);
        }

        _queryRepository
            .GetChildrenByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(new List<Category> { child });

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(
                child.Id,
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryChildrenByParentIdQuery(parentId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle();

        result.Data[0].IsRemoved.Should().BeTrue();

        result.Data[0].Actions.CanRestore.Should()
            .Be(expectedCanRestore);

        await _queryRepository.Received(1).GetByAsync(
            parentId,
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
        const long parentId = 100;

        var child = new Category(_builder.Build().Title, parentId);
        var parent = _builder.Build();

        _queryRepository
            .GetChildrenByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(new List<Category> { child });

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(
                child.Id,
                Arg.Any<CancellationToken>())
            .Returns(hasActiveChildren);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryChildrenByParentIdQuery(parentId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle();

        result.Data[0].Actions.CanRemove.Should()
            .Be(expectedCanRemove);

        await _queryRepository.Received(1).HasActiveChildrenAsync(
            child.Id,
            Arg.Any<CancellationToken>());
    }
}