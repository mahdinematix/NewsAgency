using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class FirstNameConversion : ValueConverter<FirstName, string>
{
    public FirstNameConversion() : base(c => c.Value, c => FirstName.FromString(c))
    {
    }
}