using FluentAssertions;
using NewsAgency.Application.AuthorServices.QueryHandlers.GetAll;
using NewsAgency.Application.Contracts.Author.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.QueryHandlers.GetAll;

public class GetAllAuthorsQueryHandlerTests
{
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly GetAllAuthorsQueryHandler _queryHandler;
    private readonly AuthorTestBuilder _builder;

    public GetAllAuthorsQueryHandlerTests()
    {
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _queryHandler = new GetAllAuthorsQueryHandler(_queryRepository);
        _builder = new AuthorTestBuilder();
    }


    [Fact]
    public async Task Should_Return_Success_Result_And_All_Of_Authors()
    {
        // Arrange
        var firstAuthor = _builder.Build();
        var secondAuthor = _builder.Build();

        var Authors = new List<Author>
        {
            firstAuthor,
            secondAuthor
        };

        _queryRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Authors);

        // Act
        var result = await _queryHandler.Handle(
            new GetAllAuthorsQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCountGreaterThanOrEqualTo(2);
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_And_Empty_List_When_No_Authors_Exist()
    {
        // Arrange
        _queryRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Author>());

        // Act
        var result = await _queryHandler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Repository_Returns_Null()
    {
        // Arrange
        _queryRepository.GetAllAsync(Arg.Any<CancellationToken>()).ReturnsNull();

        // Act
        var result = await _queryHandler.Handle(new GetAllAuthorsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());

    }
}