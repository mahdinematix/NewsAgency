using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Factories;
using NewsAgency.Infra.Data.EFCore.CommandRepositories;
using NewsAgency.Infra.Data.EFCore.QueryRepositories;
using NewsAgency.Infra.Data.EFCore.Tests.Integration.ClassFixtures;
using System.Transactions;
using FluentAssertions;

namespace NewsAgency.Infra.Data.EFCore.Tests.Integration.QueryRepositories;

public class AuthorQueryRepositoryTests : IClassFixture<RealDatabaseFixture>
{
    private readonly AuthorQueryRepository _queryRepository;
    private readonly AuthorCommandRepository _commandRepository;
    private readonly AuthorTestBuilder _builder;
    public AuthorQueryRepositoryTests(RealDatabaseFixture fixture)
    {
        _queryRepository = new AuthorQueryRepository(fixture.Context);
        _commandRepository = new AuthorCommandRepository(fixture.Context);
        _builder = new AuthorTestBuilder();
    }

    [Fact]
    public async Task Should_Return_An_Author_By_FirstName()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        FirstName firstName = new("Mahdi");
        var author = _builder.WithFirstName(firstName).Build();
        await _commandRepository.CreateAsync(author, CancellationToken.None);

        //act
        var actual = await _queryRepository.GetByFirstNameAsync(author.FirstName.Value, CancellationToken.None);

        //assert
        actual.Should().NotBeNull();
        actual.FirstName.Should().Be(firstName);
    }

    [Fact]
    public async Task Should_Return_An_Author_By_LastName()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        LastName lastName = new("Nemati");
        var author = _builder.WithLastName(lastName).Build();
        await _commandRepository.CreateAsync(author, CancellationToken.None);

        //act
        var actual = await _queryRepository.GetByLastNameAsync(author.LastName.Value, CancellationToken.None);

        //assert
        actual.Should().NotBeNull();
        actual.LastName.Should().Be(lastName);
    }

    [Fact]
    public async Task Should_Return_An_Author_By_Email()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        Email email = new("MahdiNematix@gmail.com");
        var author = _builder.WithEmail(email).Build();
        await _commandRepository.CreateAsync(author, CancellationToken.None);
        await _commandRepository.SaveChangesAsync(CancellationToken.None);


        //act
        var actual = await _queryRepository.GetByEmailAsync(author.Email.Value, CancellationToken.None);

        //assert
        actual.Should().NotBeNull();
        actual.Email.Should().Be(email);
    }

    [Fact]
    public async Task Should_Return_An_Author_By_PhoneNumber()
    {
        //arrange
        using var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled);
        PhoneNumber phoneNumber = new("00989381980312");
        var author = _builder.WithPhoneNumber(phoneNumber).Build();
        await _commandRepository.CreateAsync(author, CancellationToken.None);

        //act
        var actual = await _queryRepository.GetByPhoneNumberAsync(author.PhoneNumber.Value, CancellationToken.None);

        //assert
        actual.Should().NotBeNull();
        actual.PhoneNumber.Should().Be(phoneNumber);
    }
}

