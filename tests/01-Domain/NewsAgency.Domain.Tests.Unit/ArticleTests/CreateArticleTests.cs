using FluentAssertions;
using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.ArticleTests;

public class CreateArticleTests
{
    [Fact]
    public void Should_Create_Article_With_Valid_Properties()
    {
        //arrange
        long id = 1;
        var title =new ArticleTitle("title");
        var content = new ArticleContent("content123");
        long authorId = 1;
        long categoryId = 1;

        //act
        var article = new Article(id,title,content,authorId,categoryId);

        //assert
        article.Id.Should().Be(id);
        article.Title.Should().Be(title);
        article.Content.Should().Be(content);
        article.AuthorId.Should().Be(authorId);
        article.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Should_Create_Article_As_Draft()
    {
        //arrange
        long id = 1;
        var title = new ArticleTitle("title");
        var content = new ArticleContent("content123");
        long authorId = 1;
        long categoryId = 1;

        //act
        var article = new Article(id, title, content, authorId, categoryId);

        //assert
        article.Status.Should().Be(ArticleStatus.Draft);
    }

    

}

