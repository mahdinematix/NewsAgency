using FluentAssertions;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetAll;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Entities;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetAll;

public class GetAllCategoriesQueryHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly GetAllCategoriesQueryHandler _queryHandler;
    private readonly CategoryTestBuilder _builder;

    public GetAllCategoriesQueryHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _queryHandler = new GetAllCategoriesQueryHandler(_queryRepository);
        _builder = new CategoryTestBuilder();
    }

    [Fact]
    public async Task Should_Return_Success_Result_And_All_Of_Categories()
    {
        // Arrange
        var firstCategory = _builder.Build();
        var secondCategory = _builder.Build();

        var categories = new List<Category>
        {
            firstCategory,
            secondCategory
        };

        _queryRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        // Act
        var result = await _queryHandler.Handle(
            new GetAllCategoriesQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(2);
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_And_Empty_List_When_No_Categories_Exist()
    {
        // Arrange
        _queryRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Category>());

        // Act
        var result = await _queryHandler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        //arrange
        _queryRepository.GetAllAsync(Arg.Any<CancellationToken>()).ReturnsNull();

        //act
        var result = await _queryHandler.Handle(new GetAllCategoriesQuery(), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Parent_Does_Not_Exist()
    {
        // Arrange
        const long parentId = 100;

        var child = new Category(_builder.Build().Title, parentId);

        var categories = new List<Category>
        {
            child
        };

        _queryRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        _queryRepository
            .GetByAsync(parentId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            new GetAllCategoriesQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(
            parentId,
            Arg.Any<CancellationToken>());

        await _queryRepository
            .DidNotReceive()
            .HasActiveChildrenAsync(
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
            .GetAllAsync(Arg.Any<CancellationToken>())
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
            new GetAllCategoriesQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle();

        result.Data[0].Actions.CanRestore.Should()
            .Be(expectedCanRestore);

        await _queryRepository.Received(1).GetByAsync(
            parentId,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Calculate_CanRestore_Independently_For_Each_Category()
    {
        // Arrange
        const long firstParentId = 100;
        const long secondParentId = 200;

        var firstChild = new Category(
            _builder.Build().Title,
            firstParentId);

        var secondChild = new Category(
            _builder.Build().Title,
            secondParentId);

        firstChild.Remove(hasActiveChildren: false);
        secondChild.Remove(hasActiveChildren: false);

        var firstParent = _builder.Build();

        var secondParent = _builder.Build();
        secondParent.Remove(hasActiveChildren: false);

        var categories = new List<Category>
        {
            firstChild,
            secondChild
        };

        _queryRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(categories);

        _queryRepository
            .GetByAsync(firstParentId, Arg.Any<CancellationToken>())
            .Returns(firstParent);

        _queryRepository
            .GetByAsync(secondParentId, Arg.Any<CancellationToken>())
            .Returns(secondParent);

        _queryRepository
            .HasActiveChildrenAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await _queryHandler.Handle(
            new GetAllCategoriesQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);

        result.Data[0].Actions.CanRestore.Should().BeTrue();
        result.Data[1].Actions.CanRestore.Should().BeFalse();

        await _queryRepository.Received(1).GetByAsync(
            firstParentId,
            Arg.Any<CancellationToken>());

        await _queryRepository.Received(1).GetByAsync(
            secondParentId,
            Arg.Any<CancellationToken>());
    }
}