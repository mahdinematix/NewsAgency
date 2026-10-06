using System.Text.RegularExpressions;
using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.AuthorAgg.ValueObjects;

public class Email : BaseValueObject<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException(Messages.ValueCannotBeNull);
        }

        if (value.Length > 255)
        {
            throw new InvalidValueObjectStateException(
                Messages.ValueFailureToObserveMaxLength);
        }

        if (!IsValidEmail(value))
        {
            throw new InvalidValueObjectStateException(
                Messages.ValueFailureToObserveEmailFormat);
        }

        Value = value;
    }

    private static bool IsValidEmail(string value)
    {
        return Regex.IsMatch(
            value,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}