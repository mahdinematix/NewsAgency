using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.CategoryAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class CategoryTitleConversion : ValueConverter<CategoryTitle, string>
{
    public CategoryTitleConversion() : base(c => c.Value, c => CategoryTitle.FromString(c))
    {
    }
}