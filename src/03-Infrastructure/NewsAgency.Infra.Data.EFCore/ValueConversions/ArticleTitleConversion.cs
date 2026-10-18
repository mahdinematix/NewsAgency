using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.ArticleAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class ArticleTitleConversion : ValueConverter<ArticleTitle, string>
{
    public ArticleTitleConversion() : base(c => c.Value, c => ArticleTitle.FromString(c))
    {
    }
}