using FluentAssertions;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class AuthorQueryRepositoryTests : IClassFixture<RealDatabaseFixture>
{
    private readonly RealDatabaseFixture _fixture;
    private readonly AuthorTestBuilder _builder;

    public AuthorQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _fixture = fixture;
        _builder = new AuthorTestBuilder();
    }

    [Fact]
    public async Task Should_Return_An_Author_By_FirstName()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository = new AuthorQueryRepository(context);
        var commandRepository = new AuthorCommandRepository(context);

        FirstName firstName = new("Mahdi");

        var author = _builder
            .WithFirstName(firstName)
            .Build();

        await commandRepository.CreateAsync(
            author,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetByFirstNameAsync(
            author.FirstName.Value,
            CancellationToken.None);

        // Assert
        actual.Should().NotBeNull();
        actual.FirstName.Should().Be(firstName);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_An_Author_By_LastName()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository = new AuthorQueryRepository(context);
        var commandRepository = new AuthorCommandRepository(context);

        LastName lastName = new("Nemati");

        var author = _builder
            .WithLastName(lastName)
            .Build();

        await commandRepository.CreateAsync(
            author,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetByLastNameAsync(
            author.LastName.Value,
            CancellationToken.None);

        // Assert
        actual.Should().NotBeNull();
        actual.LastName.Should().Be(lastName);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_An_Author_By_Email()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository = new AuthorQueryRepository(context);
        var commandRepository = new AuthorCommandRepository(context);

        Email email = new("MahdiNematix@gmail.com");

        var author = _builder
            .WithEmail(email)
            .Build();

        await commandRepository.CreateAsync(
            author,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetByEmailAsync(
            author.Email.Value,
            CancellationToken.None);

        // Assert
        actual.Should().NotBeNull();
        actual.Email.Should().Be(email);

        // Teardown
        await transaction.RollbackAsync();
    }

    [Fact]
    public async Task Should_Return_An_Author_By_PhoneNumber()
    {
        // Arrange
        await using var context = _fixture.CreateContext();

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        var queryRepository = new AuthorQueryRepository(context);
        var commandRepository = new AuthorCommandRepository(context);

        PhoneNumber phoneNumber = new("00989381980312");

        var author = _builder
            .WithPhoneNumber(phoneNumber)
            .Build();

        await commandRepository.CreateAsync(
            author,
            CancellationToken.None);

        await commandRepository.SaveChangesAsync(
            CancellationToken.None);

        // Act
        var actual = await queryRepository.GetByPhoneNumberAsync(
            author.PhoneNumber.Value,
            CancellationToken.None);

        // Assert
        actual.Should().NotBeNull();
        actual.PhoneNumber.Should().Be(phoneNumber);

        // Teardown
        await transaction.RollbackAsync();
    }
}