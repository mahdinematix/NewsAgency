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
    private readonly RealDatabaseFixture _fixture;
    private readonly ArticleTestBuilder _builder;

    public GenericCommandRepositoryTests(RealDatabaseFixture fixture)
    {
        _fixture = fixture;
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Create_Entity()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var commandRepository =
            new GenericCommandRepository<Article, long>(context);

        var queryRepository =
            new GenericQueryRepository<Article, long>(context);

        var article = _builder.Build();

        // Act
        await commandRepository.CreateAsync(
            article,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var id = article.Id;

        // Assert
        var actual = await queryRepository.GetByAsync(
            id,
            CancellationToken.None);

        actual.Should().Be(article);

        // Teardown
        await transaction.RollbackAsync();
    }
}