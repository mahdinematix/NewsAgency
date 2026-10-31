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

        var parent = new CategoryTestBuilder()
            .WithParentId(500)
            .Build();

        var query = new GetCategoryParentByChildIdQuery(childId);

        _queryRepository
            .GetByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(child);

        _queryRepository
            .GetParentByAsync(childId, Arg.Any<CancellationToken>())
            .Returns(parent);

        // Act
        var result = await _queryHandler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Title.Should().Be(parent.Title.Value);
        result.Data.IsRemoved.Should().Be(parent.IsRemoved);
        result.Data.ParentId.Should().Be(parent.ParentId);
        await _queryRepository.Received(1).GetByAsync(childId, Arg.Any<CancellationToken>());
        await _queryRepository.Received(1).GetParentByAsync(childId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_With_Null_Data_When_Category_Has_No_Parent()
    {
        // Arrange
        const long childId = 100;

        var child = _builder.Build();
        var query = new GetCategoryParentByChildIdQuery(childId);

        _queryRepository.GetByAsync(childId, Arg.Any<CancellationToken>()).Returns(child);

        _queryRepository.GetParentByAsync(childId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(
            query,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeNull();
        await _queryRepository.Received(1)
            .GetByAsync(childId, Arg.Any<CancellationToken>());
        await _queryRepository.Received(1)
            .GetParentByAsync(childId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_NotFound_When_Child_Category_Does_Not_Exist()
    {
        // Arrange
        const long childId = 100;

        var query = new GetCategoryParentByChildIdQuery(childId);

        _queryRepository.GetByAsync(childId, Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();

        await _queryRepository.Received(1).GetByAsync(childId, Arg.Any<CancellationToken>());

        await _queryRepository.DidNotReceive().GetParentByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
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
        await _queryRepository.DidNotReceive()
            .GetByAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>());
        await _queryRepository.DidNotReceive()
            .GetParentByAsync(
                Arg.Any<long>(),
                Arg.Any<CancellationToken>());
    }
}