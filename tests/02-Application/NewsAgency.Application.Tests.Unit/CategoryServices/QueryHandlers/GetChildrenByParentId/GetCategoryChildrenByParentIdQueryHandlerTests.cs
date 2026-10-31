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
        //arrange
        var parent = _builder.Build();
        var child1 = _builder.WithParentId(parent.Id).Build();
        var child2 = _builder.WithParentId(parent.Id).Build();
        var children = new List<Category>
        {
            child1, child2
        };
        _queryRepository.GetChildrenByAsync(parent.Id, Arg.Any<CancellationToken>()).Returns(children);

        //act
        var result =
            await _queryHandler.Handle(new GetCategoryChildrenByParentIdQuery(parent.Id), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(2);
        await _queryRepository.Received(1).GetChildrenByAsync(parent.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        //act
        var result = await _queryHandler.Handle(null, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        result.Data.Should().BeNull();
        await _queryRepository.DidNotReceive().GetChildrenByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        //arrange
        var parent = _builder.Build();
        _queryRepository.GetChildrenByAsync(parent.Id, Arg.Any<CancellationToken>()).ReturnsNull();

        //act
        var result =
            await _queryHandler.Handle(new GetCategoryChildrenByParentIdQuery(parent.Id), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetChildrenByAsync(parent.Id, Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Should_Return_Success_Result_With_Empty_List_When_Parent_Has_No_Children()
    {
        // Arrange
        var parent = _builder.Build();

        _queryRepository.GetChildrenByAsync(parent.Id, Arg.Any<CancellationToken>()).Returns(new List<Category>());

        // Act
        var result = await _queryHandler.Handle(new GetCategoryChildrenByParentIdQuery(parent.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEmpty();

        await _queryRepository.Received(1).GetChildrenByAsync(parent.Id, Arg.Any<CancellationToken>());
    }
}