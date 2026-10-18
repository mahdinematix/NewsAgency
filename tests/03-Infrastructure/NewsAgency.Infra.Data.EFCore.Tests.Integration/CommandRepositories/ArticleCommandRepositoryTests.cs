using FluentAssertions;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;
using System.Transactions;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.CommandRepositories;

public class ArticleCommandRepositoryTests
    : IClassFixture<RealDatabaseFixture>
{
    private readonly ArticleCommandRepository _commandRepository;
    private readonly ArticleQueryRepository _queryRepository;
    private readonly ArticleTestBuilder _builder;

    public ArticleCommandRepositoryTests(RealDatabaseFixture fixture)
    {
        _commandRepository = new ArticleCommandRepository(fixture.Context);
        _queryRepository = new ArticleQueryRepository(fixture.Context);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Create_Article()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);
        var article = _builder.Build();

        // Act
        var id = await _commandRepository.CreateAsync(article, CancellationToken.None);

        // Assert
        var actual = await _queryRepository.GetByAsync(id);
        actual.Should().Be(article);
    }

    [Fact]
    public async Task Should_Return_Created_Article_Id()
    {
        // Arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);
        var article = _builder.Build();

        // Act
        var id = await _commandRepository.CreateAsync(article, CancellationToken.None);

        // Assert
        id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_Delete_Article_By_Id()
    {
        //arrange
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            TransactionScopeAsyncFlowOption.Enabled);
        var article = _builder.Build();
        var id = await _commandRepository.CreateAsync(article, CancellationToken.None);

        //act
        await _commandRepository.Delete(id, CancellationToken.None);

        //assert
        var expected = await _queryRepository.GetByAsync(id);
        expected.Should().BeNull();
    }
}