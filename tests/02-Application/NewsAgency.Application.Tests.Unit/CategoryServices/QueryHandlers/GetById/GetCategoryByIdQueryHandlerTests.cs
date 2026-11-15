using FluentAssertions;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetById;
using NewsAgency.Application.Common;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetById;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly GetCategoryByIdQueryHandler _queryHandler;
    private readonly CategoryTestBuilder _builder;

    public GetCategoryByIdQueryHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _queryHandler = new GetCategoryByIdQueryHandler(_queryRepository);
        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_Category_When_Pass_Id()
    {
        // Arrange
        var category = new Category(_builder.Build().Title);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryByIdQuery(category.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Data.Id.Should().Be(category.Id);
        result.Data.Title.Should().Be(category.Title.Value);
        result.Data.ParentId.Should().BeNull();
        result.Data.IsRemoved.Should().BeFalse();

        result.Data.Actions.CanRemove.Should().BeTrue();
        result.Data.Actions.CanRestore.Should().BeFalse();

        await _queryRepository.Received(1).GetByAsync(
            category.Id,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(1).HasActiveChildrenAsync(
            category.Id,
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

        await _queryRepository.DidNotReceive().GetByAsync(
            Arg.Any<long>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        // Arrange
        const long categoryId = 10;

        _queryRepository
            .GetByAsync(categoryId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryByIdQuery(categoryId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            categoryId,
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

        var category = new Category(
            _builder.Build().Title,
            parentId);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryByIdQuery(category.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            category.Id,
            Arg.Any<CancellationToken>());

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

        var category = new Category(
            _builder.Build().Title,
            parentId);

        category.Remove(hasActiveChildren: false);

        var parent = new Category(_builder.Build().Title);

        if (parentIsRemoved)
        {
            parent.Remove(hasActiveChildren: false);
        }

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .Returns(parent);

        _queryRepository
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryByIdQuery(category.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Data.IsRemoved.Should().BeTrue();

        result.Data.Actions.CanRestore.Should()
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
        var category = new Category(_builder.Build().Title);

        _queryRepository
            .GetByAsync(category.Id, Arg.Any<CancellationToken>())
            .Returns(category);

        _queryRepository
            .HasActiveChildrenAsync(
                category.Id,
                Arg.Any<CancellationToken>())
            .Returns(hasActiveChildren);

        // Act
        var result = await _queryHandler.Handle(
            new GetCategoryByIdQuery(category.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Data.Actions.CanRemove.Should()
            .Be(expectedCanRemove);

        await _queryRepository.Received(1).HasActiveChildrenAsync(
            category.Id,
            Arg.Any<CancellationToken>());
    }
}