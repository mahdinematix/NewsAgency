using FluentAssertions;
using NewsAgency.Application.AuthorServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Factories;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.Create;

public class CreateAuthorCommandHandlerTests
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly CreateAuthorCommandHandler _commandHandler;
    private readonly AuthorTestBuilder _builder;

    public CreateAuthorCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IAuthorCommandRepository>();
        _commandHandler = new CreateAuthorCommandHandler(_commandRepository);
        _builder = new AuthorTestBuilder();
    }

    [Fact]
    public async Task Should_Create_And_Return_Success_Result_When_Command_Is_Valid()
    {
        //arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati" , "MahdiNematix@gmail.com", "00989381980312");


        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();

        await _commandRepository.Received(1).CreateAsync(Arg.Any<Author>(), CancellationToken.None);
        await _commandRepository.Received(1).SaveChangesAsync(CancellationToken.None);

    }

    [Fact]
    public async Task Should_Pass_Command_Data_To_Repository()
    {
        // arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", "MahdiNematix@gmail.com", "00989381980312");

        // act
        await _commandHandler.Handle(command, CancellationToken.None);

        // assert
        await _commandRepository.Received(1).CreateAsync(
            Arg.Is<Author>(author =>
                author.FirstName.Value == command.FirstName &&
                author.LastName.Value == command.LastName &&
                author.Email.Value == command.Email &&
                author.PhoneNumber.Value == command.PhoneNumber),
            CancellationToken.None);
    }


    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        // act
        var result = await _commandHandler.Handle(
            null!,
            CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();

        await _commandRepository.DidNotReceive()
            .CreateAsync(
                Arg.Any<Author>(),
                Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_FirstName_Is_Invalid(int length)
    {
        //arrange
        var invalidFirstName = new string('a', length); ;
        var command = new CreateAuthorCommand(invalidFirstName, "Nemati", "MahdiNematix@gmail.com", "00989381980312");

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_LastName_Is_Invalid(int length)
    {
        //arrange
        var invalidLastName = new string('a', length); ;
        var command = new CreateAuthorCommand("Mahdi", invalidLastName, "MahdiNematix@gmail.com", "00989381980312");

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test@.com")]
    [InlineData("test")]
    [InlineData("@example")]
    [InlineData("@example.com")]
    [InlineData("test@example")]
    [InlineData("test @example.com")]
    public async Task Should_Return_Failed_Result_When_Email_Is_Invalid(string email)
    {
        //arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", email, "00989381980312");

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0912345678a")]
    [InlineData("091234567-")]
    [InlineData("091234567 ")]
    public async Task Should_Return_Failed_Result_When_PhoneNumber_Is_Invalid(string phoneNumber)
    {
        //arrange
        var command = new CreateAuthorCommand("Mahdi", "Nemati", "MahdiNematix@gmail.com", phoneNumber);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(9)]
    [InlineData(15)]
    public async Task Should_Return_Failed_Result_When_PhoneNumber_Length_Is_Invalid(int length)
    {
        //arrange
        string value = new string('1', length);
        var command = new CreateAuthorCommand("Mahdi", "Nemati", "MahdiNematix@gmail.com", value);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

}

