using System.Linq.Expressions;
using FluentAssertions;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangePhoneNumber;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.AuthorAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangePhoneNumber;

public class ChangeAuthorPhoneNumberCommandHandlerTests
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly ChangeAuthorPhoneNumberCommandHandler _commandHandler;
    private readonly AuthorTestBuilder _builder;

    public ChangeAuthorPhoneNumberCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IAuthorCommandRepository>();
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _commandHandler = new ChangeAuthorPhoneNumberCommandHandler(_commandRepository, _queryRepository);
        _builder = new AuthorTestBuilder();
    }
    [Fact]
    public async Task Should_Change_Author_PhoneNumber()
    {
        //arrange
        var author = _builder.Build();
        string newPhoneNumber = "009898989898";
        var command = new ChangeAuthorPhoneNumberCommand(author.Id, newPhoneNumber);
        _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        author.PhoneNumber.Value.Should().Be(newPhoneNumber);
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        //act
        var result = await _commandHandler.Handle(null!, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryRepository.DidNotReceive().GetByAsync(Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(9)]
    [InlineData(15)]
    public async Task Should_Return_Failed_Result_When_PhoneNumber_Length_Is_Invalid(int length)
    {
        //arrange
        var author = _builder.Build();
        string phoneNumber = new string('1', length);
        var command = new ChangeAuthorPhoneNumberCommand(author.Id, phoneNumber);
        _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("0912345678a")]
    [InlineData("091234567-")]
    [InlineData("091234567 ")]
    public async Task Should_Return_Failed_Result_When_PhoneNumber_Format_Is_Invalid(string phoneNumber)
    {
        //arrange
        var author = _builder.Build();
        var command = new ChangeAuthorPhoneNumberCommand(author.Id, phoneNumber);
        _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Record_Not_Found()
    {
        // Arrange
        const long authorId = 100;
        var command = new ChangeAuthorPhoneNumberCommand(
            authorId,
            "009898989898");

        _queryRepository
            .GetByAsync(authorId, Arg.Any<CancellationToken>())
            .ReturnsNull();

        // Act
        var result = await _commandHandler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_PhoneNumber_Of_Author_To_Change_Exist()
    {
        //arrange
        const long authorId = 200;
        string samePhoneNumber = "00989388899665";
        var author = _builder.WithPhoneNumber(new PhoneNumber(samePhoneNumber)).Build();
        var command = new ChangeAuthorPhoneNumberCommand(
            authorId, samePhoneNumber);
        _queryRepository
            .GetByAsync(authorId, Arg.Any<CancellationToken>())
            .Returns(author);
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Author, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);


        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Conflict);
        await _queryRepository.Received(1)
            .Exists(
                Arg.Any<Expression<Func<Author, bool>>>(),
                Arg.Any<CancellationToken>());
        await _commandRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}