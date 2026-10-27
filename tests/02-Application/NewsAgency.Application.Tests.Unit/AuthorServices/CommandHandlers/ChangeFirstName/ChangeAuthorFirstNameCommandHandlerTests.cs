using FluentAssertions;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangeFirstName;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Factories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangeFirstName;

public class ChangeAuthorFirstNameCommandHandlerTests
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly ChangeAuthorFirstNameCommandHandler _commandHandler;
    private readonly AuthorTestBuilder _builder;

    public ChangeAuthorFirstNameCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IAuthorCommandRepository>();
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _commandHandler = new ChangeAuthorFirstNameCommandHandler(_commandRepository, _queryRepository);
        _builder = new AuthorTestBuilder();
    }
    [Fact]
    public async Task Should_Change_Author_FirstName()
    {
        //arrange
        var author = _builder.Build();
        string newFirstName = "new first name";
        var command = new ChangeAuthorFirstNameCommand(author.Id, newFirstName);
        _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        author.FirstName.Value.Should().Be(newFirstName);
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        //act
        var result = await _commandHandler.Handle(null!, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryRepository.DidNotReceive().GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_FirstName_Is_Invalid(int length)
    {
        //arrange
        var author = _builder.Build();
        string fistname = new string('a', length);
        var command = new ChangeAuthorFirstNameCommand(author.Id, fistname);
        _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Author_Does_Not_Exist()
    {
        // Arrange
        const long authorId = 100;
        var command = new ChangeAuthorFirstNameCommand(
            authorId,
            "new First Name");

        _queryRepository
            .GetByAsync(authorId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}