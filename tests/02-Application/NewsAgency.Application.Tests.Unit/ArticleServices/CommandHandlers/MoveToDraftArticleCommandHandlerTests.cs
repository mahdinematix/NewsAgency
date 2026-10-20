using FluentAssertions;
using NewsAgency.Application.ArticleServices.CommandHandlers;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Article.Repositories;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers;

public class MoveToDraftArticleCommandHandlerTests
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;
    private readonly MoveToDraftArticleCommandHandler _commandHandler;
    private readonly ArticleTestBuilder _builder;

    public MoveToDraftArticleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IArticleCommandRepository>();
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _commandHandler = new MoveToDraftArticleCommandHandler(_commandRepository, _queryRepository);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Move_To_Draft_Article_When_Status_Is_Rejected()
    {
        //arrange
        var article = _builder.Build();
        article.Reject();
        _queryRepository.GetByAsync(article.Id, Arg.Any<CancellationToken>()).Returns(article);
        var command = new MoveToDraftArticleCommand(article.Id);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
        result.IsSuccess.Should().BeTrue();
        await _commandRepository.Received(1)
            .SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Should_Move_To_Draft_Article_When_Status_Is_Archived()
    {
        //arrange
        var article = _builder.Build();
        article.Publish();
        article.Archive();
        _queryRepository.GetByAsync(article.Id, Arg.Any<CancellationToken>()).Returns(article);
        var command = new MoveToDraftArticleCommand(article.Id);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
        result.IsSuccess.Should().BeTrue();
        await _commandRepository.Received(1)
            .SaveChangesAsync(CancellationToken.None);
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
        var command = new MoveToDraftArticleCommand(55557);
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
        var command = new MoveToDraftArticleCommand(article.Id);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        article.Status.Should().Be(ArticleStatus.Published);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}