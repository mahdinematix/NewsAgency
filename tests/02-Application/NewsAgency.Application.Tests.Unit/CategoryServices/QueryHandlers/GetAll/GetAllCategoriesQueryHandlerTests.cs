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
}