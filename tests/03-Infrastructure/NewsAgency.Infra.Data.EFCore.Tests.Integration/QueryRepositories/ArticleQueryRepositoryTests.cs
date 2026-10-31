using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class ArticleQueryRepositoryTests : IClassFixture<RealDatabaseFixture>
{
    private readonly RealDatabaseFixture _fixture;
    private readonly ArticleTestBuilder _builder;

    public ArticleQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _fixture = fixture;
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Return_An_Article_By_Title()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository = new ArticleQueryRepository(context);
        var commandRepository = new ArticleCommandRepository(context);

        ArticleTitle title = new("New Title");

        var article = _builder
            .WithTitle(title)
            .Build();

        await commandRepository.CreateAsync(
            article,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetByAsync(
            article.Title.Value,
            CancellationToken.None);

        // Assert
        actual.Title.Should().Be(title);

        // Teardown
        await transaction.RollbackAsync();
    }
}