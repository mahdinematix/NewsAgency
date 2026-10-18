using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class LastNameConversion : ValueConverter<LastName, string>
{
    public LastNameConversion() : base(c => c.Value, c => LastName.FromString(c))
    {
    }
}