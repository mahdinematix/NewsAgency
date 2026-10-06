using NewsAgency.Domain.Common;

namespace NewsAgency.Domain.Tests.Unit.TestDoubles;

public class TestEntity : BaseEntity
{
    public TestEntity(long id)
    {
        Id = id;
    }
}

public class AnotherTestEntity : BaseEntity
{
    public AnotherTestEntity(long id)
    {
        Id = id;
    }
}

