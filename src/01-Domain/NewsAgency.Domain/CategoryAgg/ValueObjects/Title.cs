using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.CategoryAgg.ValueObjects;

public class Title :BaseValueObject<Title>
{
    public string Value { get; }

    public Title(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException(Messages.ValueCannotBeNull);
        }

        if (value.Length < 2)
        {
            throw new InvalidValueObjectStateException(Messages.ValueFailureToObserveMinLength);
        }
        if (value.Length > 50)
        {
            throw new InvalidValueObjectStateException(Messages.ValueFailureToObserveMaxLength);
        }
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}

