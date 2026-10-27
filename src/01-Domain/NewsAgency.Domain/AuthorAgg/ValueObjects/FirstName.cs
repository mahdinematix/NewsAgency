using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.AuthorAgg.ValueObjects;

public class FirstName : BaseValueObject<FirstName>
{
    public string Value { get; }
    public int MaxLength { get; } = 50;
    public int MinLength { get; } = 2;
    public static FirstName FromString(string value) => new(value);


    public FirstName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException(Messages.ValueCannotBeNull);
        }
        if (value.Length < MinLength)
        {
            throw new InvalidValueObjectStateException(Messages.ValueFailureToObserveMinLength);
        }
        if (value.Length > MaxLength)
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