using System.Transactions;
using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class ArticleQueryRepositoryTests : IClassFixture<RealDatabaseFixture>
{
    private readonly ArticleQueryRepository _queryRepository;
    private readonly ArticleCommandRepository _commandRepository;
    private readonly ArticleTestBuilder _builder;
    public ArticleQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _queryRepository = new ArticleQueryRepository(fixture.Context);
        _commandRepository = new ArticleCommandRepository(fixture.Context);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Return_List_Of_Articles()
    {
        //act
        var articles = await _queryRepository.GetAllAsync(CancellationToken.None);

        //assert
        articles.Should().HaveCountGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Should_Return_An_Article_By_Id()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required,  TransactionScopeAsyncFlowOption.Enabled);
        var article = _builder.Build();
        await _commandRepository.CreateAsync(article, CancellationToken.None);

        //act
        var actual = await _queryRepository.GetByAsync(article.Id, CancellationToken.None);

        //assert
        actual.Should().Be(article);
    }

    [Fact]
    public async Task Should_Return_An_Article_By_Title()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        ArticleTitle title = new("NewTitle");
        var article = _builder.WithTitle(title).Build();
        await _commandRepository.CreateAsync(article, CancellationToken.None);

        //act
        var actual = await _queryRepository.GetByAsync(article.Title.Value, CancellationToken.None);

        //assert
        actual.Title.Should().Be(title);
    }
}