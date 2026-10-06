using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.AuthorAgg.ValueObjects;

public class PhoneNumber : BaseValueObject<PhoneNumber>
{
    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException(Messages.ValueCannotBeNull);
        }
        if (value.Length < 10)
        {
            throw new InvalidValueObjectStateException(
                Messages.ValueFailureToObserveMinLength);
        }
        if (value.Length > 14)
        {
            throw new InvalidValueObjectStateException(
                Messages.ValueFailureToObserveMaxLength);
        }
        if (!value.All(char.IsDigit))
        {
            throw new InvalidValueObjectStateException(
                Messages.ValueMustContainOnlyNumbers);
        }

        Value = value;
    }

    

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}