using FluentAssertions;
using NewsAgency.Application.ArticleServices.CommandHandlers.Archive;
using NewsAgency.Application.Contracts.Article.Commands;
using NewsAgency.Application.Contracts.Common;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.Repositories;
using NewsAgency.Domain.Tests.Unit.Builders;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace NewsAgency.Application.Tests.Unit.ArticleServices.CommandHandlers.Archive;

public class ArchiveArticleCommandHandlerTests
{
    private readonly IArticleCommandRepository _commandRepository;
    private readonly IArticleQueryRepository _queryRepository;
    private readonly ArchiveArticleCommandHandler _commandHandler;
    private readonly ArticleTestBuilder _builder;

    public ArchiveArticleCommandHandlerTests()
    {
        _commandRepository = Substitute.For<IArticleCommandRepository>();
        _queryRepository = Substitute.For<IArticleQueryRepository>();
        _commandHandler = new ArchiveArticleCommandHandler(_commandRepository, _queryRepository);
        _builder = new ArticleTestBuilder();
    }

    [Fact]
    public async Task Should_Archive_Article_When_Status_Is_Published()
    {
        //arrange
        var article = _builder.Build();
        article.Publish();
        _queryRepository.GetByAsync(article.Id, Arg.Any<CancellationToken>()).Returns(article);
        var command = new ArchiveArticleCommand(article.Id);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        article.Status.Should().Be(ArticleStatus.Archived);
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
        result.ErrorCode.Should().Be(OperationErrorCode.Validation);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Record_Not_Found()
    {
        //arrange
        var command = new ArchiveArticleCommand(55557);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).ReturnsNull();

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.NotFound);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Status_Is_Rejected()
    {
        //arrange
        var article = _builder.Build();
        article.Reject();
        var command = new ArchiveArticleCommand(article.Id);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.LogicError);
        article.Status.Should().Be(ArticleStatus.Rejected);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Return_Failed_Result_When_Status_Is_Draft()
    {
        //arrange
        var article = _builder.Build();
        var command = new ArchiveArticleCommand(article.Id);
        _queryRepository.GetByAsync(command.Id, Arg.Any<CancellationToken>()).Returns(article);

        //act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        //assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be(OperationErrorCode.LogicError);
        article.Status.Should().Be(ArticleStatus.Draft);
        await _commandRepository.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}