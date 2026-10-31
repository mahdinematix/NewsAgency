using System.Linq.Expressions;
using FluentAssertions;
using NewsAgency.Application.ArticleServices.CommandHandlers.Create;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Create;

public class CreateArticleCommandHandlerTests
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;

    private readonly CreateArticleCommandHandler _commandHandler;

    public CreateArticleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IArticleCommandRepository>();
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _commandHandler = new CreateArticleCommandHandler(_commandRepository, _queryRepository);
    }

    [Fact]
    public async Task Should_Create_And_Return_Success_Result_When_Command_Is_Valid()
    {
        //arrange
        var command = new CreateArticleCommand("Article Title Test", "Content Test", 5, 3);


        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();
        await _commandRepository.Received(1).CreateAsync(Arg.Any<Article>(), CancellationToken.None);
        await _commandRepository.Received(1).SaveChangesAsync(CancellationToken.None);

    }

    [Fact]
    public async Task Should_Pass_Real_Command_Data_To_Repository()
    {
        // arrange
        var command = new CreateArticleCommand("Article Title Test", "Content Test", 5, 3);

        // act
        await _commandHandler.Handle(command, CancellationToken.None);

        // assert
        await _commandRepository.Received(1).CreateAsync(
            Arg.Is<Article>(article =>
                article.Title.Value == command.Title &&
                article.Content.Value == command.Content &&
                article.AuthorId == command.AuthorId &&
                article.CategoryId == command.CategoryId),
            CancellationToken.None);
    }


    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        // act
        var result = await _commandHandler.Handle(
            null,
            CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .CreateAsync(
                Arg.Any<Article>(),
                Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_Title_Is_Invalid(int length)
    {
        //arrange
        var invalidTitle = "A".PadLeft(length, '-');
        var command = new CreateArticleCommand(invalidTitle, "Content Test", 5, 3);

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
    [InlineData(9)]
    [InlineData(5001)]
    public async Task Should_Return_Failed_Result_When_Content_Is_Invalid(int length)
    {
        //arrange
        var invalidContent = "A".PadLeft(length, '-');
        var command = new CreateArticleCommand("Article Title Test", invalidContent, 5, 3);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Article_To_Create_Title_Exist()
    {
        //arrange
        string sameTitle = "Same Article Title";
        var command1 = new CreateArticleCommand(sameTitle, "Content Test", 5, 3);
        var command2 = new CreateArticleCommand(sameTitle, "Content Test", 5, 3);
        _queryRepository
            .Exists(
                Arg.Any<Expression<Func<Article, bool>>>(),
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
        await _commandRepository.Received(1).CreateAsync(Arg.Any<Article>(), Arg.Any<CancellationToken>());
        await _commandRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryRepository.Received(2)
            .Exists(
                Arg.Any<Expression<Func<Article, bool>>>(),
                Arg.Any<CancellationToken>());

    }

}
