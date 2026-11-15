using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class ArticleCapabilitiesTests
{
    private readonly ArticleTestBuilder _builder;

    public ArticleCapabilitiesTests()
    {
        _builder = new ArticleTestBuilder();
    }

    [Theory]
    [InlineData(ArticleStatus.Draft,
        true,
        true,
        true,
        false,
        false)]
    [InlineData(ArticleStatus.Published,
        false,
        false, 
        false, 
        true,
        false)]
    [InlineData(ArticleStatus.Rejected,
        true,
        false,
        false,
        false, 
        true)]
    [InlineData(ArticleStatus.Archived,
        false,
        false, 
        false, 
        false,
        true)]
    public void Should_Return_Correct_Capabilities_According_To_Article_Status(
        ArticleStatus status,
        bool canEdit,
        bool canPublish,
        bool canReject,
        bool canArchive,
        bool canMoveToDraft)
    {
        // Arrange
        var article = _builder.Build();

        switch (status)
        {
            case ArticleStatus.Published:
                article.Publish();
                break;

            case ArticleStatus.Rejected:
                article.Reject();
                break;

            case ArticleStatus.Archived:
                article.Publish();
                article.Archive();
                break;
        }

        //Assert
        article.Status.Should().Be(status);

        article.CanEdit.Should().Be(canEdit);
        article.CanPublish.Should().Be(canPublish);
        article.CanReject.Should().Be(canReject);
        article.CanArchive.Should().Be(canArchive);
        article.CanMoveToDraft.Should().Be(canMoveToDraft);
    }
}