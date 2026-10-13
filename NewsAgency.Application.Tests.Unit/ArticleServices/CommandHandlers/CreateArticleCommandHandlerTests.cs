using FluentAssertions;
using NewsAgency.Application.ArticleServices.CommandHandlers;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NSubstitute;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers;

public class CreateArticleCommandHandlerTests
{
    [Fact]
    public async Task Should_Create_And_Return_Success_When_Command_Is_Valid()
    {
        //arrange
        var command = new CreateArticleCommand(16, "test", "testtest123", 2, 3);
        var commandRepository = Substitute.For<IArticleCommandRepository>();
        var commandHandler = new CreateArticleCommandHandler(commandRepository);

        //act
        var result = await commandHandler.Handle(command, CancellationToken.None);

        //assert
        await commandRepository.Received(1).Create(Arg.Any<Article>(), CancellationToken.None);
        result.IsSuccess.Should().BeTrue();

    }

    [Fact]
    public async Task Should_Pass_Real_Command_Data_To_Repository()
    {
        // arrange
        var command = new CreateArticleCommand(
            16,
            "Test Article",
            "This is valid test content",
            2,
            3);

        var commandRepository =
            Substitute.For<IArticleCommandRepository>();

        var commandHandler =
            new CreateArticleCommandHandler(commandRepository);

        // act
        await commandHandler.Handle(command, CancellationToken.None);

        // assert
        await commandRepository.Received(1).Create(
            Arg.Is<Article>(article =>
                article.Id == command.Id &&
                article.Title.Value == command.Title &&
                article.Content.Value == command.Content &&
                article.AuthorId == command.AuthorId &&
                article.CategoryId == command.CategoryId),
            CancellationToken.None);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(51)]
    public async Task Should_Return_Failed_Result_When_Title_Is_Invalid(int length)
    {
        // arrange
        var command = new CreateArticleCommand(
            16,
            "A".PadLeft(length, '-'),
            "This is valid test content",
            2,
            3);

        var commandRepository =
            Substitute.For<IArticleCommandRepository>();

        var commandHandler =
            new CreateArticleCommandHandler(commandRepository);

        // act
        var result = await commandHandler.Handle(
            command,
            CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
        await commandRepository.DidNotReceive()
            .Create(
                Arg.Any<Article>(),
                Arg.Any<CancellationToken>());

    }

    [Theory]
    [InlineData(9)]
    [InlineData(5001)]
    public async Task Should_Return_Failed_Result_When_Content_Is_Invalid(int length)
    {
        // arrange
        var command = new CreateArticleCommand(
            16,
            "Test Article",
            "A".PadLeft(length, '-'),
            2,
            3);

        var commandRepository =
            Substitute.For<IArticleCommandRepository>();

        var commandHandler =
            new CreateArticleCommandHandler(commandRepository);

        // act
        var result = await commandHandler.Handle(
            command,
            CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();
        await commandRepository.DidNotReceive()
            .Create(
                Arg.Any<Article>(),
                Arg.Any<CancellationToken>());

    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Request_Is_Null()
    {
        //arrange
        var commandRepository =
            Substitute.For<IArticleCommandRepository>();

        var commandHandler =
            new CreateArticleCommandHandler(commandRepository);

        // act
        var result = await commandHandler.Handle(
            null,
            CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeFalse();

        await commandRepository.DidNotReceive()
            .Create(
                Arg.Any<Article>(),
                Arg.Any<CancellationToken>());
    }


}
