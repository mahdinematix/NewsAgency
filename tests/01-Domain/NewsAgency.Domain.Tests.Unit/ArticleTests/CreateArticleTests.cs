using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;
using NewsAgency.Domain.Tests.Unit.Builders;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class CreateArticleTests
{
    private readonly ArticleTestBuilder _articleBuilder;

    public CreateArticleTests()
    {
        _articleBuilder = new ArticleTestBuilder();
    }
    [Fact]
    public void Should_Create_Article_With_Valid_Properties()
    {
        //arrange
        var title =new ArticleTitle("categoryTitle");
        var content = new ArticleContent("content123");
        long authorId = 1;
        long categoryId = 1;

        //act
        var article = new Article(title,content,authorId,categoryId);

        //assert
        article.Title.Should().Be(title);
        article.Content.Should().Be(content);
        article.AuthorId.Should().Be(authorId);
        article.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Should_Create_Article_As_Draft()
    {
        //act
        var article = _articleBuilder.Build();

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
    }
}
