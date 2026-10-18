using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class EmailConversion : ValueConverter<Email, string>
{
    public EmailConversion() : base(c => c.Value, c => Email.FromString(c))
    {
    }
}