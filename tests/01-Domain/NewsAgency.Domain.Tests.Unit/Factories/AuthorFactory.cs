using NewsAgency.Domain.AuthorAgg.Entities;

namespace NewsAgency.Domain.Tests.Unit.Factories;

public class AuthorFactory
{
    public static Author Create()
    {
        return new Author(1, new(new string('a', 2)), new(new string('b', 2)), new("test@example.com"), new("00989123456789"));
    }

}
