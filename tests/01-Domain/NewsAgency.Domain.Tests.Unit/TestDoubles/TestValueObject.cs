using NewsAgency.Domain.Common;

namespace NewsAgency.Domain.Tests.Unit.TestDoubles;

public class TestValueObject : BaseValueObject<TestValueObject>
{
    private readonly string _value;

    public TestValueObject(string value)
    {
        _value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
    }

}
