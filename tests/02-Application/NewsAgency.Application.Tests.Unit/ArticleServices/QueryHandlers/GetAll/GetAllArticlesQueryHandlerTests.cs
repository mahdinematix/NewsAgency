using FluentAssertions;
using NewsAgency.Application.ArticleServices.QueryHandlers.GetAll;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.QueryHandlers.GetAll;

public class GetAllArticlesQueryHandlerTests
{
    private readonly IArticleQueryRepository _queryRepository;
    private readonly GetAllArticlesQueryHandler _queryHandler;
    private readonly ArticleTestBuilder _builder;

    public GetAllArticlesQueryHandlerTests()
    {
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _queryHandler = new GetAllArticlesQueryHandler(_queryRepository);
        _builder = new ArticleTestBuilder();
    }


    [Fact]
    public async Task Should_Return_Success_Result_And_All_Of_Articles()
    {
        // Arrange
        var firstArticle = _builder.Build();
        var secondArticle = _builder.Build();

        var articles = new List<Article>
        {
            firstArticle,
            secondArticle
        };

        _queryRepository
            .GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(articles);

        // Act
        var result = await _queryHandler.Handle(
            new GetAllArticlesQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Success_And_Empty_List_When_No_Articles_Exist()
    {
        // Arrange
        _queryRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(new List<Article>());

        // Act
        var result = await _queryHandler.Handle(new GetAllArticlesQuery(), CancellationToken.None);

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
        var result = await _queryHandler.Handle(new GetAllArticlesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());

    }

}