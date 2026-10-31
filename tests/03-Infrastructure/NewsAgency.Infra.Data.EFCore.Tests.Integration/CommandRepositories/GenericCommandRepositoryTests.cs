using System.Transactions;
using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.CommandRepositories;

public class GenericCommandRepositoryTests
    : IClassFixture<RealDatabaseFixture>
{
    private readonly GenericCommandRepository<Article, long> _commandRepository;
    private readonly GenericQueryRepository<Article, long> _queryRepository;
    private readonly ArticleTestBuilder _builder;

    public GenericCommandRepositoryTests(RealDatabaseFixture fixture)
    {
        _commandRepository =
            new GenericCommandRepository<Article, long>(fixture.Context);

        _queryRepository = new GenericQueryRepository<Article, long>(fixture.Context);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Create_Entity()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);

        var article = _builder.Build();

        // Act
        await _commandRepository.CreateAsync(
            article,
            CancellationToken.None);
        await _commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var id = article.Id;

        // Assert
        var actual = await _queryRepository.GetByAsync(
            id,
            CancellationToken.None);

        actual.Should().Be(article);
    }

}