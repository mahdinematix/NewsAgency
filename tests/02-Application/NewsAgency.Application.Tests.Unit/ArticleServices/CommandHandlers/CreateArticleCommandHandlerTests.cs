using FluentAssertions;
using NewsAgency.Application.ArticleServices.CommandHandlers;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Repositories;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers;

public class CreateArticleCommandHandlerTests
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly CreateArticleCommandHandler _commandHandler;
    private readonly ArticleTestBuilder _builder;

    public CreateArticleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IArticleCommandRepository>();
        _commandHandler = new CreateArticleCommandHandler(_commandRepository);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Create_And_Return_Success_Result_When_Command_Is_Valid()
    {
        //arrange
        var command = new CreateArticleCommand("CategoryTitle Test", "Content Test", 5, 3);


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
        var command = new CreateArticleCommand("CategoryTitle Test", "Content Test", 5, 3);

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
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }


}
