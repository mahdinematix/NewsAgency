using NewsAgency.Domain.ArticleAgg.Entities;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Domain.Tests.Unit.Factories;

public class ArticleFactory
{
    public static Article Create()
    {
        return new Article(1, new ArticleTitle("a".PadLeft(5, '-')), new ArticleContent("a".PadLeft(10, '-')), 1, 1);
    }
}
