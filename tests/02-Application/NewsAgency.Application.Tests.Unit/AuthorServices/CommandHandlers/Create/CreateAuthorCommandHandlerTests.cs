using FluentAssertions;
using NewsAgency.Application.AuthorServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Factories;
using NSubstitute;
using System.Linq.Expressions;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Domain.AuthorAgg.Entities;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.Create;

public class CreateAuthorCommandHandlerTests
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly CreateAuthorCommandHandler _commandHandler;
    private readonly AuthorTestBuilder _builder;

    public CreateAuthorCommandHandlerTests()
    {
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _commandRepository = Substitute.For<IAuthorCommandRepository>();
        _commandHandler = new CreateAuthorCommandHandler(_commandRepository, _queryRepository);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Author_To_Create_Title_Exist()
    {
        //arrange
        string sameEmail = "SameEmail@test.test";
        var command1 = new CreateAuthorCommand("test firstName", "test lastName", sameEmail,"00989388899665");
        var command2 = new CreateAuthorCommand("test firstName", "test lastName", sameEmail, "00989388899665");
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Author, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(false),
                Task.FromResult(true));

        //act
        var result1 = await _commandHandler.Handle(command1, CancellationToken.None);
        var result2 = await _commandHandler.Handle(command2, CancellationToken.None);


        //assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeFalse();
        result2.ErrorCode.Should().Be(OperationErrorCode.Conflict);
        await _commandRepository.Received(1).CreateAsync(Arg.Any<Author>(), Arg.Any<CancellationToken>());
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryRepository.Received(2)
            .Exists(
                Arg.Any<Expression<Func<Author, bool>>>(),
                Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Author_To_Create_PhoneNumber_Exist()
    {
        //arrange
        string samePhoneNumber = "00989388899665";
        var command1 = new CreateAuthorCommand("Test First Name", "Test Last Name", "SameEmail@test.test", samePhoneNumber);
        var command2 = new CreateAuthorCommand("Test First Name", "Test Last Name", "SameEmail@test.test", samePhoneNumber);
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Author, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(false),
                Task.FromResult(true));

        //act
        var result1 = await _commandHandler.Handle(command1, CancellationToken.None);
        var result2 = await _commandHandler.Handle(command2, CancellationToken.None);


        //assert
        result1.IsSuccess.Should().BeTrue();
        result2.IsSuccess.Should().BeFalse();
        await _commandRepository.Received(1).CreateAsync(Arg.Any<Author>(), Arg.Any<CancellationToken>());
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

    }

}

