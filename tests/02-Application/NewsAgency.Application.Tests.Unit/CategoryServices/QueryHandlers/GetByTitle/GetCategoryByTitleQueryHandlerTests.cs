using FluentAssertions;
using NewsAgency.Application.CategoryServices.QueryHandlers.GetByTitle;
using NewsAgency.Application.Contracts.Category.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.CategoryAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.CategoryServices.QueryHandlers.GetByTitle;

public class GetCategoryByTitleQueryHandlerTests
{
    private readonly ICategoryQueryRepository _queryRepository;
    private readonly GetCategoryByTitleQueryHandler _queryHandler;
    private readonly CategoryTestBuilder _builder;

    public GetCategoryByTitleQueryHandlerTests()
    {
        _queryRepository = Substitute.For<ICategoryQueryRepository>();
        _queryHandler = new GetCategoryByTitleQueryHandler(_queryRepository);
        _builder = new CategoryTestBuilder();
    }


    [Fact]
    public async Task Should_Return_Success_Result_And_Category_When_Pass_Id()
    {
        // Arrange
        var category = _builder.Build();
        _queryRepository.GetByAsync(category.Title.Value, Arg.Any<CancellationToken>()).Returns(category);

        // Act
        var result = await _queryHandler.Handle(new GetCategoryByTitleQuery(category.Title.Value), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Title.Should().Be(category.Title.Value);
        result.Data.ParentId.Should().Be(category.ParentId.HasValue ? category.ParentId : null);
        result.Data.IsRemoved.Should().Be(category.IsRemoved);
        await _queryRepository.Received(1).GetByAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

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
        await _queryRepository.DidNotReceive().GetByAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        //arrange
        var Category = _builder.Build();

        //act
        var result = await _queryHandler.Handle(new GetCategoryByTitleQuery(Category.Title.Value), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetByAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

    }

}