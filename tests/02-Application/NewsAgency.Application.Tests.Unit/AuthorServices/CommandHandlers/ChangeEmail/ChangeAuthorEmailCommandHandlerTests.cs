using FluentAssertions;
using NewsAgency.Application.AuthorServices.CommandHandlers.ChangeEmail;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Author.Commands;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Entities;
using NewsAgency.Domain.AuthorAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Factories;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System.Linq.Expressions;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Application.Tests.Unit.AuthorServices.CommandHandlers.ChangeEmail;

public class ChangeAuthorEmailCommandHandlerTests
{
    private readonly IAuthorCommandRepository _commandRepository;
    private readonly IAuthorQueryRepository _queryRepository;
    private readonly ChangeAuthorEmailCommandHandler _commandHandler;
    private readonly AuthorTestBuilder _builder;

    public ChangeAuthorEmailCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IAuthorCommandRepository>();
        _queryRepository = Substitute.For<IAuthorQueryRepository>();
        _commandHandler = new ChangeAuthorEmailCommandHandler(_commandRepository, _queryRepository);
        _builder = new AuthorTestBuilder();
    }
    [Fact]
    public async Task Should_Change_Author_Email()
    {
        //arrange
        var author = _builder.Build();
        string newEmail = "newEmail@edited.co";
        var command = new ChangeAuthorEmailCommand(author.Id, newEmail);
        _queryRepository.GetByAsync(author.Id, Arg.Any<CancellationToken>()).Returns(author);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        author.Email.Value.Should().Be(newEmail);
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
        var author = _builder.Build();
        var command = new ChangeAuthorEmailCommand(author.Id, email);
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
        var command = new ChangeAuthorEmailCommand(
            authorId,
            "newEmail@example.com");

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

    [Fact]
    public async Task Should_Return_Failed_Result_When_Email_Of_Author_To_Change_Exist()
    {
        //arrange
        string sameEmail = "SameEmail@test.test";
        var article = _builder.WithEmail(new Email(sameEmail)).Build();
        var command = new ChangeAuthorEmailCommand(
            article.Id, sameEmail);
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Author, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(false),
                Task.FromResult(true));

        //act
        var result1 = await _commandHandler.Handle(command, CancellationToken.None);


        //assert
        result1.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}