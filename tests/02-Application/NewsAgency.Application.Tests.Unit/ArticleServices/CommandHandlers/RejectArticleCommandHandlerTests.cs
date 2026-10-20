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

public class RejectArticleCommandHandlerTests
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;
    private readonly RejectArticleCommandHandler _commandHandler;
    private readonly ArticleTestBuilder _builder;

    public RejectArticleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IArticleCommandRepository>();
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _commandHandler = new RejectArticleCommandHandler(_commandRepository, _queryRepository);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Reject_Article_When_Status_Is_Draft()
    {
        //arrange
        var article = _builder.Build();
        _queryRepository.GetByAsync(article.Id, Arg.Any<CancellationToken>()).Returns(article);
        var command = new RejectArticleCommand(article.Id);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        article.Status.Should().Be(ArticleStatus.Rejected);
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
        var command = new RejectArticleCommand(55557);
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
        var command = new RejectArticleCommand(article.Id);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        article.Status.Should().Be(ArticleStatus.Published);
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
        var command = new RejectArticleCommand(article.Id);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        article.Status.Should().Be(ArticleStatus.Archived);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}