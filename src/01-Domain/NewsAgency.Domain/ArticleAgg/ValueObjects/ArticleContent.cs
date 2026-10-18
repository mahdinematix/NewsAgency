using NewsAgency.Domain.Common;
using NewsAgency.Domain.Exceptions;

namespace NewsAgency.Domain.ArticleAgg.ValueObjects;

public class ArticleContent : BaseValueObject<ArticleContent>
{
    public string Value { get; }
    public static ArticleContent FromString(string value) => new (value);

    public ArticleContent(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidValueObjectStateException(Messages.ValueCannotBeNull);
        }

        if (value.Length < 10)
        {
            throw new InvalidValueObjectStateException(Messages.ValueFailureToObserveMinLength);
        }
        if (value.Length > 5000)
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

