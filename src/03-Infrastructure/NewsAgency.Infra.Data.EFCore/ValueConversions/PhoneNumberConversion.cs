using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NewsAgency.Domain.AuthorAgg.ValueObjects;

namespace NewsAgency.Infra.Data.EFCore.ValueConversions;

public class PhoneNumberConversion : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberConversion() : base(c => c.Value, c => PhoneNumber.FromString(c))
    {
    }
}