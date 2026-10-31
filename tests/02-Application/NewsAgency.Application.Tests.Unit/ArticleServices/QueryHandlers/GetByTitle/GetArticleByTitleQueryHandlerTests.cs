using FluentAssertions;
using NewsAgency.Application.ArticleServices.QueryHandlers.GetByTitle;
using NewsAgency.Application.Contracts.Article.Queries;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Utilities.Extensions;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.QueryHandlers.GetByTitle;

public class GetArticleByTitleQueryHandlerTests
{
    private readonly IArticleQueryRepository _queryRepository;
    private readonly GetArticleByTitleQueryHandler _queryHandler;
    private readonly ArticleTestBuilder _builder;

    public GetArticleByTitleQueryHandlerTests()
    {
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _queryHandler = new GetArticleByTitleQueryHandler(_queryRepository);
        _builder = new ArticleTestBuilder();
    }


    [Fact]
    public async Task Should_Return_Success_Result_And_Article_When_Pass_Id()
    {
        // Arrange
        var article = _builder.Build();
        _queryRepository.GetByAsync(article.Title.Value, Arg.Any<CancellationToken>()).Returns(article);

        // Act
        var result = await _queryHandler.Handle(new GetArticleByTitleQuery(article.Title.Value), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Title.Should().Be(article.Title.Value);
        result.Data.Content.Should().Be(article.Content.Value);
        result.Data.Status.Should().Be(article.Status.GetEnumDescription());
        result.Data.AuthorId.Should().Be(article.AuthorId);
        result.Data.CategoryId.Should().Be(article.CategoryId);
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
        var article = _builder.Build();

        //act
        var result = await _queryHandler.Handle(new GetArticleByTitleQuery(article.Title.Value), CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        result.Data.Should().BeNull();
        await _queryRepository.Received(1).GetByAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

    }

}