using FluentAssertions;
using NewsAgency.Application.ArticleServices.CommandHandlers.Edit;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Edit;

public class EditArticleCommandHandlerTests
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;
    private readonly EditArticleCommandHandler _commandHandler;
    private readonly ArticleTestBuilder _builder;

    public EditArticleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IArticleCommandRepository>();
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _commandHandler = new EditArticleCommandHandler(_commandRepository, _queryRepository);
        _builder = new ArticleTestBuilder();
    }


    [Fact]
    public async Task Should_Edit_Article_And_Return_Success_When_Command_Is_Valid()
    {
        //arrange
        var article = _builder.Build();
        var command = new EditArticleCommand(article.Id, "Title Test edited", "Content Test edited", 1, 1);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeTrue();

        article.Title.Value.Should().Be(command.Title);
        article.Content.Value.Should().Be(command.Content);
        article.AuthorId.Should().Be(command.AuthorId);
        article.CategoryId.Should().Be(command.CategoryId);

        await _commandRepository.Received(1).SaveChangesAsync(CancellationToken.None);
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
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Record_Not_Found()
    {
        //arrange
        var article = _builder.Build();
        var command = new EditArticleCommand(55557, "Title Test edited", "Content Test edited", 1, 1);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).ReturnsNull();

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Status_Is_Published()
    {
        //arrange
        var article = _builder.Build();
        article.Publish();
        var command = new EditArticleCommand(article.Id, "Title Test edited", "Content Test edited", 1, 1);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Status_Is_Archived()
    {
        //arrange
        var article = _builder.Build();
        article.Publish();
        article.Archive();
        var command = new EditArticleCommand(article.Id, "Title Test edited", "Content Test edited", 1, 1);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

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
    public async Task Should_Return_Failed_Result_When_Title_Is_Invalid(int length)
    {
        //arrange
        var article = _builder.Build();
        var invalidTitle = "A".PadLeft(length, '-');
        var command = new EditArticleCommand(article.Id, invalidTitle, "Content Test edited", 1, 1);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

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
        var article = _builder.Build();
        var invalidContent = "A".PadLeft(length, '-');
        var command = new EditArticleCommand(article.Id, "Title Test edited", invalidContent , 1, 1);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

}
