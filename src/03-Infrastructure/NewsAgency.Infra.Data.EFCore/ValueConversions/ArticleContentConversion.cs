using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class ArticleContentConversion : ValueConverter<ArticleContent, string>
{
    public ArticleContentConversion() : base(c => c.Value, c => ArticleContent.FromString(c))
    {
    }
}