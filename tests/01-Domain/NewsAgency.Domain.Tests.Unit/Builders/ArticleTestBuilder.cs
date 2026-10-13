using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.Builders;

public class ArticleTestBuilder
{
    private long _id = 1;
    private ArticleTitle _title = new("a".PadLeft(5, '-'));
    private ArticleContent _content = new("b".PadLeft(10, '-'));
    private long _authorId = 1;

    private long _categoryId = 1;

    public Article Build()
    {
        return new Article(_id, _title, _content, _authorId, _categoryId);
    }

    public ArticleTestBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
}
