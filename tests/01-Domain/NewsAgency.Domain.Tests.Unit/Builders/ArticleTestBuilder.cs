using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.Builders;

public class ArticleTestBuilder
{
    private readonly long _id = 1;
    private ArticleTitle Title = new ArticleTitle("a".PadLeft(5, '-'));
    private ArticleContent Content = new ArticleContent("a".PadLeft(10, '-'));
    private readonly long _authorId = 1;
    private readonly long _categoryId = 1;

    public Article Build()
    {
        return new Article(_id, Title, Content, _authorId, _categoryId);
    }

    public ArticleTestBuilder WithTitle(ArticleTitle title)
    {
        Title = title;
        return this;
    }
    public ArticleTestBuilder WithContent(ArticleContent content)
    {
        Content = content;
        return this;
    }
}
