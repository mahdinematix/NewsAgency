using System.Transactions;
using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class GenericQueryRepositoryTests
    : IClassFixture<RealDatabaseFixture>
{
    private readonly GenericQueryRepository<Article, long> _queryRepository;
    private readonly GenericCommandRepository<Article, long> _commandRepository;
    private readonly ArticleTestBuilder _builder;

    public GenericQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _queryRepository =
            new GenericQueryRepository<Article, long>(fixture.Context);

        _commandRepository =
            new GenericCommandRepository<Article, long>(fixture.Context);

        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Return_List_Of_Entities()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var article = _builder.Build();

        await _commandRepository.CreateAsync(
            article,
            CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);


        // Act
        var articles = await _queryRepository.GetAllAsync(
            CancellationToken.None);

        // Assert
        articles.Should().Contain(article);
    }

    [Fact]
    public async Task Should_Return_Entity_By_Id()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var article = _builder.Build();

        await _commandRepository.CreateAsync(
            article,
            CancellationToken.None);

        await _commandRepository.SaveChangesAsync(CancellationToken.None);

        var id = article.Id;

        // Act
        var actual = await _queryRepository.GetByAsync(
            id,
            CancellationToken.None);

        // Assert
        actual.Id.Should().Be(id);
        actual.Should().Be(article);
    }

    [Fact]
    public async Task Should_Return_Null_When_Entity_Does_Not_Exist()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        const long nonExistingId = 999999999L;

        // Act
        var actual = await _queryRepository.GetByAsync(
            nonExistingId,
            CancellationToken.None);

        // Assert
        actual.Should().BeNull();
    }
}