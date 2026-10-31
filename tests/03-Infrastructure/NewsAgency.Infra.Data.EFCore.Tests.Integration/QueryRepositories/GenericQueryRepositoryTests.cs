using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class GenericQueryRepositoryTests
    : IClassFixture<RealDatabaseFixture>
{
    private readonly RealDatabaseFixture _fixture;
    private readonly ArticleTestBuilder _builder;

    public GenericQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _fixture = fixture;
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Return_List_Of_Entities()
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

        await commandRepository.CreateAsync(
            article,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var articles = await queryRepository.GetAllAsync(
            CancellationToken.None);

        // Assert
        articles.Should().ContainSingle(x => x.Id == article.Id);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_Entity_By_Id()
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

        await commandRepository.CreateAsync(
            article,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        var id = article.Id;

        // Act
        var actual = await queryRepository.GetByAsync(
            id,
            CancellationToken.None);

        // Assert
        actual.Should().NotBeNull();
        actual!.Id.Should().Be(id);
        actual.Title.Should().Be(article.Title);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_Null_When_Entity_Does_Not_Exist()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        var queryRepository =
            new GenericQueryRepository<Article, long>(context);

        const long nonExistingId = long.MaxValue;

        // Act
        var actual = await queryRepository.GetByAsync(
            nonExistingId,
            CancellationToken.None);

        // Assert
        actual.Should().BeNull();
    }

    [Fact]
    public async Task Should_Return_True_When_Entity_Exist()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var commandRepository =
            new GenericCommandRepository<Article, long>(context);

        var queryRepository =
            new GenericQueryRepository<Article, long>(context);

        const string sameTitle = "same title";

        var article = _builder
            .WithTitle(new ArticleTitle(sameTitle))
            .Build();

        await commandRepository.CreateAsync(
            article,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var isExists = await queryRepository.Exists(
            x => x.Title == new ArticleTitle(sameTitle),
            CancellationToken.None);

        // Assert
        isExists.Should().BeTrue();

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_False_When_Entity_Does_Not_Exist()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        var queryRepository =
            new GenericQueryRepository<Article, long>(context);

        var nonExistingTitle = new ArticleTitle(
            $"missing-{Guid.NewGuid():N}");

        // Act
        var isExists = await queryRepository.Exists(
            x => x.Title == nonExistingTitle,
            CancellationToken.None);

        // Assert
        isExists.Should().BeFalse();
    }
}